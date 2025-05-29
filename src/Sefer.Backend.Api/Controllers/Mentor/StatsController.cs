namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class StatsController(IServiceProvider provider) : UserController(provider)
{
    [HttpGet("/mentor/stats")]
    [ProducesResponseType(typeof(MentorStats), 200)]
    public async Task<ActionResult<MentorStats>> GetMentorStats()
    {
        var mentor = await GetCurrentUser();
        if (mentor.Role != UserRoles.Mentor) return Forbid();

        var stats = await Send(new GetMentorStatsRequest(mentor.Id));
        return Json(stats);
    }
}