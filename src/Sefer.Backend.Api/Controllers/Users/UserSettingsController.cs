using Microsoft.Extensions.Configuration;
using Sefer.Backend.Api.Models.Users;

namespace Sefer.Backend.Api.Controllers.Users;

[Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
public class UserSettingsController(IServiceProvider provider, IPasswordService passwordService)
    : Abstract.ProfileController(provider, passwordService)
{
    private readonly IConfiguration _configuration = provider.GetService<IConfiguration>();

    [HttpGet("/user/additional-settings")]
    public async Task<ActionResult> GetSettings()
    {
        // try to load the user that is requesting his settings (404)
        var user = await GetCurrentUser();
        if (user == null) return NotFound();
        
        var settings = await Send(new GetUserSettingsRequest(user.Id));
        var view = settings.ToDictionary(v => v.KeyName, v => v.Value);
        return Json(view);
    }
    
    [HttpPost("/user/additional-settings")]
    public async Task<ActionResult> SaveSettings([FromBody] UserSettingsPostModel post)
    {
        if (ModelState.IsValid == false) return BadRequest();

        // try to load the user that is updating his settings (404)
        var user = await GetCurrentUser();
        if (user == null) return NotFound();

        // Now save the additional settings
        var allowed = _configuration.GetSection("UserSettings").Get<string[]>();
        var request = new UpdateUserSettingsRequest(user.Id, post.Settings, allowed);
        var settingsSaved = await Send(request);
        return settingsSaved ? StatusCode(202) : StatusCode(500);
    }
}

