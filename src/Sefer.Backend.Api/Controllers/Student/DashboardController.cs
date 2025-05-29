using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Requests.Rewards;
using Sefer.Backend.Api.Views.Student;
using Student_EnrollmentView = Sefer.Backend.Api.Views.Student.EnrollmentView;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class DashboardController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    /// <summary>
    /// Generates an overview with the latest activity information of the user
    /// </summary>
    /// <returns>A list with the latest activity information of the user</returns>
    /// <response code="403">The user has no student or user role</response>
    /// <response code="200">A list with the latest activity information of the user</response>
    [HttpGet("/student/activity-summary")]
    [ProducesResponseType(typeof(List<ActivityView>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<List<ActivityView>>> GetActivityLog()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        var logins = await Send(new GetLoginsOfUserRequest(student.Id, 10));
        var enrollments = await Send(new GetEnrollmentsOfStudentRequest(student.Id, 10));
        var submissions = await Send(new GetSubmittedLessonsRequest(student.Id, 10));

        var loginViews = logins.Select(l => new ActivityView(l));
        var enrollmentsViews = enrollments.Select(e => new ActivityView(e));
        var submissionsViews = submissions.Select(s => new ActivityView(s, _fileStorageService));
        var activity = loginViews.Union(enrollmentsViews).Union(submissionsViews);

        var view = activity.OrderByDescending(l => l.Time).ToList();
        return Json(view);
    }

    [HttpGet("/student/active-enrollment")]
    [ProducesResponseType(typeof(UserView), 200)]
    public async Task<IActionResult> GetEnrollment()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        var enrollment = await Send(new GetActiveEnrollmentOfStudentRequest(student.Id, true));
        if (enrollment == null) return NotFound();
        var view = new Student_EnrollmentView(enrollment, _fileStorageService);
        return Json(view);
    }

    [HttpGet("/student/reward-enrollments/{code}")]
    [ProducesResponseType(typeof(UserView), 200)]
    public async Task<IActionResult> GetCurrentEnrollmentsForRewarding(string code)
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();

        var reward = await Send(new GetRewardByCodeKeyRequest(code));
        if (reward == null) return BadRequest();

        var nextTargets = await Send(new GetNextTargetsRequest(student.Id, reward.Id));
        if (nextTargets == null || nextTargets.Count != 0 == false) return NoContent();

        // Please note: if this call is slow, the query could be updated to count rather than to return a list
        var enrollments = await Send(new GetRewardedEnrollmentsRequest(student.Id, reward.Id));
        var view = nextTargets.Select(t => new RewardView(t, enrollments.Count));
        return Json(view);
    }
}