using Sefer.Backend.Api.Views.Mentor;

namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class DashboardController(IServiceProvider serviceProvider) : UserController(serviceProvider)
{
    /// <summary>
    /// Returns the list of recent activity of the mentor
    /// </summary>
    [HttpGet("/mentor/activity-summary")]
    [ProducesResponseType(typeof(List<MentorActivityView>), 200)]
    public async Task<ActionResult<List<MentorActivityView>>> GetActivityLog()
    {
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        var logins = await Send(new GetLoginsOfUserRequest(mentor.Id, 10));

        var view = logins.Select(l => new MentorActivityView(l)).ToList();
        return Json(view);
    }

    /// <summary>
    /// Returns the number of submission still to review by the mentor
    /// </summary>
    [HttpGet("/mentor/unprocessed-submissions")]
    [Obsolete("This method will be removed if the final ui will no longer use it")]
    public async Task<ActionResult> GetUnprocessedSubmissionsMentor()
    {
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        var count = await Send(new GetSubmissionsForReviewCountRequest(mentor.Id));
        if (count == null) return BadRequest();
        return Json(new { UnprocessedSubmissions = count });
    }
}