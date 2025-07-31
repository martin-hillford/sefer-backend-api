using Validator = Sefer.Backend.Api.Services.Security.TwoAuth.Validator;

namespace Sefer.Backend.Api.Controllers.Public;

public class LogonController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IUserAuthenticationService _userAuthenticationService = serviceProvider.GetService<IUserAuthenticationService>();

    private readonly ICryptographyService _cryptoService = serviceProvider.GetService<ICryptographyService>();
    
    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();
    
    /// <summary>
    /// Logs the user into his account
    /// </summary>
    /// <param name="post"></param>
    /// <response code="200">Login accepted</response>
    /// <response code="400">The user is blocked (because of misconduct)</response>
    /// <response code="403">The user cannot log on to his account since it is not yet approved</response>
    /// <response code="401">The user cannot log on because the username or password are incorrect</response>
    /// <response code="202">The password and username are correct, require two-factor auth</response>
    /// <response code="418">The password and username are correct, factor auth code is not correct</response>
    /// <response code="200">The logon is a success, provides the user access to his account</response>
    /// <response code="500">There is something wrong with the logon request, check security settings</response>
    [HttpPost("/public/user/logon")]
    public Task<ActionResult> WebLogon([FromBody] LogonPostModel post) => Logon(post, false);

    [HttpPost("/public/user/app-logon")]
    public Task<ActionResult> AppLogon([FromBody] LogonPostModel post) => Logon(post, true);

    private async Task<ActionResult> Logon([FromBody] LogonPostModel post, bool isAppLogin)
    {
        if (post == null) return await LogAndReturnUnauthorized(null);

        var user = await Send(new GetUserByEmailRequest(post.Username));
        if (user == null) return await LogAndReturnUnauthorized(post);

        var result = await _userAuthenticationService.SignOn(post.Username, post.Password);
        await LogonLog(result, post.Username, user.Id);

        return result switch
        {
            SignOnResult.Blocked => BadRequest(),
            SignOnResult.Unapproved => await Unapproved(post, user),
            SignOnResult.IncorrectSignIn => Unauthorized(),
            SignOnResult.Success => await HandleCorrectLogon(post, user, isAppLogin),
            _ => StatusCode(500)
        };
    }

    private async Task<ActionResult> Unapproved(LogonPostModel post, User student)
    {
        try
        {
            if (post.ResendActivation == true)
            {
                var language = student.PreferredInterfaceLanguage;
                await _notificationService.SendCompleteRegistrationNotificationAsync(student, language);
            }
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch(Exception) { }
        return Forbid();
    }
    
    private async Task<ActionResult> LogAndReturnUnauthorized(LogonPostModel post, SignOnResult result = SignOnResult.NoUser)
    {
        await LogonLog(result, post?.Username ?? string.Empty, null);
        return Unauthorized();
    }

    private async Task<ActionResult> HandleCorrectLogon(LogonPostModel post, User user, bool isAppLogin)
    {
        // If two-factor authentication is not enabled, then the user gets access
        if (user.TwoFactorAuthEnabled == false) return await GrantAccess(post, user, isAppLogin);

        // if this was only the logon with a code return 202 code letting the user know he should log on with two-factor auth
        if (post.Code == null) return Accepted();

        // The user has submitted a code. Validate it
        var validator = new Validator();
        var hasAccess = validator.Validate(user.TwoFactorAuthSecretKey, post.Code.Value);

        // If the user has entered the correct code, he gets access else denied
        return hasAccess ? await GrantAccess(post, user, isAppLogin) : StatusCode(418);
    }

    private async Task<ActionResult> GrantAccess(LogonPostModel post, User user, bool isAppLogin)
    {
        // Ensure to set the cookies.
        _userAuthenticationService.SetUserCookies(user, isAppLogin);
        
        // If this is an admin logon, the site and region are irrelevant
        if (IsAdminLogon(post, user)) return await SendUserToken(user, isAppLogin);

        // If this is a logon for the app, then the user can granted access since
        // the app is capable of handling any region
        if (isAppLogin) return await SendUserToken(user, true);

        // Let's check if the current site is capable of handling the current site
        // the user is logon to. If the current site cannot handle the region,
        // then the user should be redirected to log on to his/hers primary site
        var (userRegion, userSite) = await Send(new GetPrimaryRegionAndSiteRequest(user.Id));
        var currentSite = await Send(new GetSiteByNameRequest(post.Site));

        return userRegion.ContainsSite(currentSite) 
            ? await SendUserToken(user, false)
            : GrantAccessChangeSite(user, userRegion, userSite);
    }

    private static bool IsAdminLogon(LogonPostModel post, User user)
    {
        return post.Site.StartsWith("admin") && (user.Role == UserRoles.Admin || user.Role == UserRoles.CourseMaker);
    }

    private async Task<ActionResult> SendUserToken(User user, bool isAppLogin)
    {
        var securityOptions = GetService<IOptions<SecurityOptions>>()?.Value;
        if (securityOptions == null) return StatusCode(500);

        var expiration = isAppLogin
            ? DateTime.UtcNow.AddYears(2000)
            : DateTime.UtcNow.AddHours(securityOptions.TokenDurationInt);

        var tokenGenerator = GetService<ITokenGenerator>();
        var token = tokenGenerator.CreateToken(user.Id, user.Role.ToString(), expiration);
        var userSettings = await Send(new GetUserSettingsRequest(user.Id));
        var view = new LogonView(user, userSettings, expiration, token);
        _userAuthenticationService.SetPrivateFileServiceCookies();

        return UserSettingsHelper.ToJson(view, userSettings, "user");
    }

    /// <summary>
    /// This method can be used to redirect a certain user to it's primary site
    /// </summary>
    private JsonResult GrantAccessChangeSite(User user, IRegion region, ISite site)
    {
        var queryString = _cryptoService.TimeProtectedQueryString("u", user.Id.ToString());
        var destination = $"{site.SiteUrl}/logon-via-redirect?region={region.Id}&{queryString}";
        return Json(new { redirect = true, destination, region, site = new LogonSiteView(site) });
    }

    private async Task LogonLog(SignOnResult result, string username, int? userId)
    {
        var logEntry = new LoginLogEntry
        {
            LogTime = DateTime.UtcNow,
            Result = result,
            Username = username?.ToLower(),
            IpAddress = Request.GetClientIpAddress() ?? "",
            AcceptedLanguage = Request.GetAcceptedLanguages() ?? "",
            BrowserToken = Request.GetBrowserToken(true) ?? "",
            Path = Request.GetPathName() ?? "",
            UserAgent = Request.GetUserAgent() ?? "",
            UserId = userId
        };

        await Send(new AddLoginLogEntryRequest(logEntry));
    }
}