namespace Sefer.Backend.Api.Controllers.Public;

public class SiteRedirectController : BaseController
{
    private readonly ICryptographyService _cryptographyService;

    private readonly IUserAuthenticationService _userAuthenticationService;

    private readonly SecurityOptions _securityOptions;

    public SiteRedirectController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _cryptographyService = GetService<ICryptographyService>();
        _userAuthenticationService = GetService<IUserAuthenticationService>();
        _securityOptions = GetService<IOptions<SecurityOptions>>()?.Value;
    }

    [HttpPost("/users/logon-via-redirect")]
    public async Task<ActionResult> SiteRedirectLogon([FromBody] SiteRedirectLogonPostModel model)
    {
        var data = model.UserId.ToString();
        const int duration = 300;
        var isValid = _cryptographyService.IsTimeProtectedQueryString(data, model.Random, model.Date, model.Hash, duration);
        if (!isValid) return Unauthorized();

        var user = await Send(new GetUserByIdRequest(model.UserId));
        if (user == null) return Unauthorized();

        var expiration = DateTime.UtcNow.AddHours(_securityOptions.TokenDurationInt);
        var tokenGenerator = GetService<ITokenGenerator>();
        var token = tokenGenerator.CreateToken(user.Id, user.Role.ToString(), expiration);
        var userSettings = await Send(new GetUserSettingsRequest(user.Id));
        var view = new LogonView(user, userSettings, expiration, token);
        _userAuthenticationService.SetPrivateFileServiceCookies();

        return UserSettingsHelper.ToJson(view, userSettings,"user");
    }
}