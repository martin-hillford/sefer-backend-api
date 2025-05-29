namespace Sefer.Backend.Api.Controllers.Public;

public class PasswordController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IPasswordService _passwordService = serviceProvider.GetService<IPasswordService>();

    private readonly ICryptographyService _cryptoService = serviceProvider.GetService<ICryptographyService>();

    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();

    [HttpPost("/public/user/password-forgot")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> ForgotPassword([FromBody] PasswordForgotPostModel post)
    {
        var user = await Send(new GetUserByEmailRequest(post.Email));
        if (user == null) return Ok();

        await _notificationService.SendPasswordForgotNotificationAsync(user, post.Language);
        return Ok();
    }

    [HttpPost("/public/user/password-reset")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult> ResetPassword([FromBody] PasswordResetPostModel post)
    {
        if (post == null || ModelState.IsValid == false) return BadRequest();

        var valid = _cryptoService.IsTimeProtectedQueryString(post.User.ToString(), post.Random, post.Date, post.Hash);
        if (valid == false) return BadRequest();

        var user = await Send(new GetUserByIdRequest(post.User));
        if (user == null) return BadRequest();

        _passwordService.UpdatePassword(user, post.Password);
        await Send(new UpdateUserRequest(user));
        await _notificationService.SendPasswordResetCompletedNotificationAsync(user, post.Language);
        return Ok();
    }
}