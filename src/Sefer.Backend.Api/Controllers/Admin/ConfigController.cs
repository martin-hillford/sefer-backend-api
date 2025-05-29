using Sefer.Backend.Api.Models.Admin.Config;
using Sefer.Backend.Api.Views.Admin.Config;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class ConfigController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/config")]
    [ProducesResponseType(typeof(ConfigView), 200)]
    public async Task<IActionResult> GetConfig()
    {
        var settings = await Send(new GetSettingsRequest());
        var view = new ConfigView(settings);
        return Json(view);
    }

    [HttpPost("/config")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> SaveConfig([FromBody] ConfigPostModel config)
    {
        if (config == null) return BadRequest();
        if (ModelState.IsValid == false) return BadRequest();
        var valid = await Send(new UpdateSettingsRequest(config.ToModel()));
        return (valid) ? StatusCode(202) : BadRequest();
    }
}