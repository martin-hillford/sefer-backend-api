using Validator = Sefer.Backend.Api.Services.Security.TwoAuth.Validator;

namespace Sefer.Backend.Api.Controllers.Public;

[Authorize]
public class AccountController(IServiceProvider serviceProvider) : Support.UserController(serviceProvider)
{
    private const int BackupKeyCount = 5;

    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();

    /// <summary>
    /// Returns if the user has two-factor-authentication enabled
    /// </summary>
    [HttpGet("/account/two-factor-authentication/is-enabled")]
    [ProducesResponseType(typeof(UserUserTwoAuthEnabledView), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult> HasUserUserTwoAuthEnabled()
    {
        var user = await Send(new GetUserByIdRequest(UserId));
        if (user == null) return Unauthorized();
        return Json(new UserUserTwoAuthEnabledView(user));
    }

    /// <summary>
    /// To set up two-factor auth as a first step, a secret key must be generated
    /// </summary>
    /// <response code="400">Two-factor auth is already setup for this account</response>
    [HttpGet("/account/two-factor-authentication/init-setup")]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(Setup), 200)]
    public async Task<ActionResult> SetupTwoFactorAuthentication()
    {
        // check if the user can be found (401)
        var user = await Send(new GetUserByIdRequest(UserId));
        if (user == null) return Unauthorized();

        // Check if the user already has set up two-factor authentication
        if (user.TwoFactorAuthEnabled) return BadRequest();

        // Generate the two-factor authentication key and qr-code
        var (_, site) = await Send(new GetPrimaryRegionAndSiteRequest(user.Id));
        var secretKey = SetupGenerator.GenerateSecretKey();
        var generator = new SetupGenerator();
        var setup = generator.Generate(site.Name, user.Name, secretKey);

        // Save the two-factor authentication in the user profile
        user.TwoFactorAuthSecretKey = secretKey;
        await Send(new UpdateSingleUserPropertyRequest(user, nameof(user.TwoFactorAuthSecretKey)));

        // Return the result
        return Json(setup);
    }

    /// <summary>
    /// To set up two-factor auth in the second step, the user has to provide proof that the setup is correct
    /// </summary>
    /// <response code="200">Two-factor auth is enabled, and backup keys are provided</response>
    /// <response code="400">Two-factor auth could not be enabled</response>
    /// <response code="401">User could not be found</response>
    /// <response code="403">The provided code is missing or invalid</response>
    [HttpPost("/account/two-factor-authentication/complete-setup")]
    [ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(403)]
    [ProducesResponseType(typeof(TwoAuthBackupKeysView), 200)]
    public async Task<ActionResult> FinishTwoFactorAuthentication([FromBody] TwoFactorAuthPostModel post)
    {
        // check if the user can be found (401)
        var user = await Send(new GetUserByIdRequest(UserId));
        if (user == null) return Unauthorized();

        // validate the user provider code
        var validator = new Validator();
        if (post == null || post.Code == 0 || validator.Validate(user.TwoFactorAuthSecretKey, post.Code) == false) return Forbid();

        // Generate 5 secret keys for the user
        var keys = new List<string>(BackupKeyCount);
        for (var index = 0; index < BackupKeyCount; index++) { keys.Add(SetupGenerator.GenerateSecretKey()); }

        // Save the result into the database
        user.TwoFactorAuthEnabled = true;
        var updated = await Send(new UpdateBackupKeysRequest(user, keys));
        if (!updated) return BadRequest();
        var enabled = await Send(new UpdateSingleUserPropertyRequest(user, nameof(user.TwoFactorAuthEnabled)));
        if (!enabled) return BadRequest();

        // The validation was oke
        await SendEmailNotification(user, true, post.Language);
        return Json(new TwoAuthBackupKeysView { Keys = keys });
    }

    [HttpPost("/account/two-factor-authentication/disable")]
    public async Task<ActionResult> DisableTwoFactorAuth([FromBody] TwoFactorAuthPostModel post)
    {
        // check if the user can be found (401)
        var user = await Send(new GetUserByIdRequest(UserId));
        if (user == null) return Unauthorized();

        // validate the user provider code
        var validator = new Validator();
        if (post == null || post.Code == 0 || validator.Validate(user.TwoFactorAuthSecretKey, post.Code) == false) return Forbid();

        // Save the result into the database
        var disabled = await Mediator.Send(new DisableTwoFactorAuthRequest(user.Id));
        if (!disabled) return BadRequest();

        // The validation was oke
        await SendEmailNotification(user, false, post.Language);
        return Ok();
    }

    [AllowAnonymous, HttpPost("/account/two-factor-authentication/emergency")]
    public async Task<ActionResult> EmergencyLogin([FromBody] EmergencyLogonModel post)
    {
        var validSignOn = await UserAuthenticationService.SignOn(post.Username, post.Password);
        if (validSignOn != SignOnResult.Success) return Unauthorized();

        var user = await Mediator.Send(new GetUserByEmailRequest(post.Username));
        if (user == null) return Unauthorized();

        var key = post.BackupKey?.ToUpper().Replace(" ", string.Empty).Replace("-", string.Empty).Trim();
        var validKey = await Mediator.Send(new IsValidBackupKeyRequest(user.Id, key));
        if (validKey == false) return Unauthorized();

        var disabled = await Mediator.Send(new DisableTwoFactorAuthRequest(user.Id));
        if (disabled == false) return Unauthorized();

        await SendEmailNotification(user, false, post.Language);
        return Ok();
    }

    private async Task SendEmailNotification(User user, bool enabled, string language)
    {
        if (enabled) await _notificationService.SendTwoFactorAuthEnabledNotificationAsync(user, language);
        else await _notificationService.SendTwoFactorAuthDisabledNotificationAsync(user, language);
    }
}