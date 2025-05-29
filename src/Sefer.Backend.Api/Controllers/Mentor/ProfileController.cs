using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Mentor;
using Sefer.Backend.Api.Views.Shared.Users;

namespace Sefer.Backend.Api.Controllers.Mentor;

[Authorize(Roles = "Mentor")]
public class ProfileController(IServiceProvider provider) : Abstract.ProfileController(provider)
{
    private readonly IAvatarService _avatarService = provider.GetService<IAvatarService>();

    [HttpGet("/mentor/settings")]
    [ProducesResponseType(typeof(MentorSettingsView), 200)]
    public async Task<ActionResult<MentorSettingsView>> GetMentorSettings()
    {
        var mentor = await GetCurrentUser();
        if (mentor.Role != UserRoles.Mentor) return Forbid();

        var settings = await Mediator.Send(new GetMentorSettingsRequest { MentorId = mentor.Id });
        if (settings == null) return NotFound();

        var view = new MentorSettingsView(mentor, settings);
        return Json(view);
    }

    [HttpPost("/mentor/settings")]
    public async Task<ActionResult> SetMentorSettings([FromBody] MentorSettingsPostModel settings)
    {
        if (settings == null) return BadRequest();
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        var data = await Send(new GetMentorSettingsRequest { MentorId = mentor.Id });
        data.AllowOverflow = settings.AllowOverflow;
        data.MaximumStudents = settings.MaximumStudents;
        data.PreferredStudents = settings.PreferredStudents;

        var settingsUpdated = await Send(new SetMentorSettingsRequest(data));
        if (!settingsUpdated) return BadRequest();

        mentor.NotificationPreference = settings.NotificationPreference;
        var mentorUpdated = await Send(new UpdateUserRequest(mentor));
        return mentorUpdated ? Accepted() : BadRequest();
    }

    [HttpGet("/mentor/profile-info")]
    [ProducesResponseType(typeof(UserView), 200)]
    public async Task<IActionResult> GetMentorInformation()
    {
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();
        var view = new ProfileInfoView(mentor, _avatarService);
        return Json(view);
    }

    [HttpPost("/mentor/profile-info")]
    [ProducesResponseType(typeof(UserView), 200)]
    [ProducesResponseType(typeof(UserView), 202)]
    public async Task<ActionResult> UpdateMentorInformation([FromBody] ProfileInfoPostModel profile)
    {
        // try to load the mentor that is updating his profile (404)
        var mentor = await GetCurrentUser();
        if (mentor is not { Role: UserRoles.Mentor }) return Forbid();

        // And use the base controller
        return await UpdateProfileInformation(profile, mentor.Id);
    }
}