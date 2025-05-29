using Sefer.Backend.Api.Models.Users;

namespace Sefer.Backend.Api.Controllers.Users;

[Authorize(Roles = "Student,User,Admin,Mentor,Supervisor")]
public class ProfileController(IServiceProvider provider) : UserController(provider)
{
    [HttpPost("/user/update-password")]
    [ProducesResponseType(202)]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordPostModel post)
    {
        // check the input (400)
        if (post == null || ModelState.IsValid == false) return BadRequest();
        if (post.ConfirmNewPassword != post.Password) return BadRequest();

        // check if the user can be found (401)
        var user = await GetCurrentUserAsync();
        if (user == null) return Unauthorized();

        // check if the user has provided the correct current password (403)
        var passwordService = GetService<IPasswordService>();
        if (passwordService.IsValidPassword(user, post.OldPassword) == false) return Forbid();

        // Update the password and return (202)
        passwordService.UpdatePassword(user, post.Password);
        await Send(new UpdateUserRequest(user));

        var notificationService = GetService<INotificationService>();
        await notificationService.SendPasswordResetCompletedNotificationAsync(user, post.Language);
        return Accepted();
    }

    [HttpPost("/user/push-notifications/token")]
    public async Task<ActionResult> SetPushNotificationToken([FromBody] PushNotificationToken post)
    {
        // check if the user can be found (401)
        var user = await GetCurrentUserAsync();
        if (user == null || post.UserId != user.Id) return Unauthorized();

        // add the token and return the result
        var saved = await Send(new AddPushNotificationTokenRequest(post));
        return saved ? Ok() : BadRequest();
    }
}