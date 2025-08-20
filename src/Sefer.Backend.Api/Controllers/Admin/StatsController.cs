using Microsoft.Extensions.Configuration;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class StatsController(IConfiguration configuration, IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/stats/{*path}"), HttpPost("/stats/{*path}")]
    public IActionResult GetStats(string path)
    {
        try
        {
            var statsApi = configuration.GetValue<string>("StatsApi");
            return Redirect($"{statsApi}/{path}");
        }
        catch (Exception) { return NotFound(); }
    }
}