using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class ProfileController(IServiceProvider provider) : Abstract.ProfileController(provider)
{
    [HttpGet("/admin/profile-info")]
    public async Task<ActionResult<UserView>> GetAdminInformation()
    {
        var admin = await GetCurrentUser();
        if (admin is not { Role: UserRoles.Admin }) return Forbid();
        var view = new UserView(admin);
        return Json(view);
    }

    [HttpPost("/admin/profile-info")]
    [ProducesResponseType(typeof(UserView), 200)]
    [ProducesResponseType(typeof(UserView), 202)]
    public async Task<ActionResult> UpdateAdminInformation([FromBody] ProfileInfoPostModel profile)
    {
        // try to load the admin that is updating his profile (404)
        var admin = await GetCurrentUser();
        if (admin is not { Role: UserRoles.Admin }) return Forbid();
        return await UpdateProfileInformation(profile, admin.Id);
    }
}