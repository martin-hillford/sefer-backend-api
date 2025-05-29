namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class LogController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/logs/mail-notifications")]
    [ProducesResponseType(typeof(List<Log>), 200)]
    public async Task<IActionResult> GetNotificationAndMailLogs([FromQuery] int take = 1000, [FromQuery] int skip = 0)
    {
        var logs = await Send(new GetNotificationAndMailLogsRequest(skip, take));
        return Json(logs);
    }
}