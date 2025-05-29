using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Models.Student.Profile;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Shared.Users;

namespace Sefer.Backend.Api.Controllers.Student;

[Authorize(Roles = "Student,User")]
public class ProfileController(IServiceProvider provider) : Abstract.ProfileController(provider)
{
    private readonly IAvatarService _avatarService = provider.GetService<IAvatarService>();
    
    [HttpGet("/student/profile-info")]
    [ProducesResponseType(typeof(UserView), 200)]
    public async Task<IActionResult> GetStudentInformation()
    {
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return Forbid();
        var view = new ProfileInfoView(student, _avatarService);
        return Json(view);
    }

    [HttpPost("/student/profile-info")]
    [ProducesResponseType(typeof(UserView), 200)]
    [ProducesResponseType(typeof(UserView), 202)]
    public async Task<ActionResult> UpdateStudentInformation([FromBody] ProfileInfoPostModel profile)
    {
        // try to load the student that is updating his profile (404)
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return NotFound();
        return await UpdateProfileInformation(profile, student.Id);
    }

    [HttpPost("/student/profile/request-delete")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DeleteAccount([FromBody] AccountDeletePostModel post)
    {
        // This method will always report a 200 message to prevent e-mail address leaking
        if (post == null) return Ok();

        var user = await GetCurrentUser();
        if (user == null) return Ok();

        await NotificationService.SendAccountDeleteConfirmationNotificationAsync(user, post.Language);
        return Ok();
    }
    
    [HttpPost("/student/settings")]
    public async Task<ActionResult> SaveSettings([FromBody] StudentSettingsPostModel settings)
    {
        if (ModelState.IsValid == false) return BadRequest();

        // try to load the student that is updating his profile (404)
        var student = await GetCurrentUser();
        if (student == null || student.IsMentor) return NotFound();
        
        // Save the settings that are saved in the user object itself
        student.NotificationPreference = settings.NotificationPreference;
        student.PreferSpokenCourses = settings.PreferSpokenCourses;
        var saved = await Send(new UpdateUserRequest(student));
        return saved ? StatusCode(202) : StatusCode(500);
    }
}