namespace Sefer.Backend.Api.Controllers.Public;

public class EnrollmentController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();

    [ProducesResponseType(typeof(CourseEnrollmentView), 200)]
    [HttpGet("/public/user/enroll/{courseId:int}")]
    [Authorize(Roles = "Student,User")]
    public async Task<IActionResult> IsEnrollableFor(int courseId)
    {
        var student = await GetCurrentUser();
        if (student == null) return Forbid();

        var course = await Send(new GetCourseByIdRequest(courseId));
        if (course == null) return NotFound();

        var canEnroll = await Send(new IsStudentEnrollableForCourseRequest(student, course));
        var hasIssue = canEnroll && await HasPersonalMentorEnrollIssue(student.Id, course.Id);
        return Json(new CourseEnrollmentView(course, canEnroll, hasIssue));
    }

    [ProducesResponseType(typeof(EnrollmentView), 200)]
    [HttpPost("/public/user/enroll")]
    [Authorize(Roles = "Student,User")]
    public async Task<ActionResult> Enroll([FromBody] EnrollmentPostModel post)
    {
        if (post == null) return BadRequest();
        var student = await GetCurrentUser();
        if (student is not { IsStudent: true }) return Forbid();

        var course = await Send(new GetPublishedCourseByIdRequest(post.CourseId));
        if (course == null) return NotFound();

        var enrollment = await Send(new EnrollRequest(student.Id, course.Id));
        if (enrollment == null) return StatusCode(500);

        if (student.Role == UserRoles.User) await Send(new UpdateUserRoleRequest(student.Id, UserRoles.Student));

        await _notificationService.SendStudentEnrolledInCourseNotificationAsync(course, enrollment);
        var view = new EnrollmentView(enrollment, course, student, enrollment.Mentor, _fileStorageService);
        return Json(view);
    }

    private async Task<bool> HasPersonalMentorEnrollIssue(int studentId, int courseId)
    {
        // If the student does not have a personal mentor, then there is no issue
        var personalMentor = await Send(new GetPersonalMentorRequest(studentId));
        if (personalMentor == null) return false;

        // If the student does have a mentor but that mentor is not
        // teaching the course then the student has an issue
        var request = new GetPersonalMentorForCourseRequest(studentId, courseId);
        var personalMentorForCourse = await Send(request);
        return personalMentorForCourse == null;
    }
}