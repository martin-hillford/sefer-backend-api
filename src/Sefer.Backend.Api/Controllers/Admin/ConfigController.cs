using Sefer.Backend.Api.Models.Admin.Config;
using Sefer.Backend.Api.Views.Admin.Config;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class ConfigController(IServiceProvider provider, IPasswordService passwordService) : BaseController(provider)
{
    [HttpGet("/settings")]
    [ProducesResponseType(typeof(ConfigView), 200)]
    public async Task<IActionResult> GetConfig()
    {
        var settings = await Send(new GetSettingsRequest());
        var view = new ConfigView(settings);
        return Ok(view);
    }

    [HttpPost("/settings")]
    [ProducesResponseType(202)]
    public async Task<ActionResult> SaveConfig([FromBody] ConfigPostModel config)
    {
        if (config == null || !ModelState.IsValid) return BadRequest();
        var valid = await Send(new UpdateSettingsRequest(config.ToModel()));
        return valid ? Accepted() : BadRequest();
    }

    [AllowAnonymous, HttpPost("/setup")]
    public async Task<ActionResult> Setup([FromBody] RegistrationPostModel model)
    {
        // If the model state is not valid, the post is valid, bad request
        if (model == null || !ModelState.IsValid) return BadRequest();
        
        // Check if there are users present in the system. If so, this function cannot process
        var userCount = await Send(new GetUsersCountRequest());
        if (userCount > 0) return Conflict("There are already users in the system.");
        
        // Check the site and the region sent to the server 
        var site = await Send(new GetSiteByNameRequest(model.Site));
        var region = await Send(new GetRegionByIdRequest(model.Region));
        if (site == null || region == null | !site.ContainsRegion(region)) 
            return Problem("Site or region don't exists or site not in region");
        
        // Create a new admin and use the password service to set the password
        // Ensure to approve the admin else he can't log on. Also ensure that the admin role
        var user = model.Create(site, region, true);
        passwordService.UpdatePassword(user, model.Password);
        user.Role = UserRoles.Admin;

        // Check if the user is valid and save it
        var isValid = await Send(new AddUserRequest(user));
        if (isValid == false) return BadRequest();
        
        // Done, report back 
        return Created();
    }
}