namespace Sefer.Backend.Api.Controllers.Admin;

public class ImpersonationController : BaseController
{
    private readonly ICryptographyService _cryptographyService;

    private readonly IUserAuthenticationService _userAuthenticationService;

    private readonly SecurityOptions _securityOptions;

    public ImpersonationController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _cryptographyService = GetService<ICryptographyService>();
        _userAuthenticationService = GetService<IUserAuthenticationService>();
        _securityOptions = GetService<IOptions<SecurityOptions>>()?.Value;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("/admin/users/get-impersonation-url")]
    public async Task<ActionResult> GetImpersonationUrl(int userId)
    {
        // Get the user and check if the user is a mentor or student
        var user = await Send(new GetUserByIdRequest(userId));
        if (user == null) return NotFound();
        if (user.Role == UserRoles.Admin || user.Role == UserRoles.CourseMaker) return BadRequest();

        // Get if the user allows impersonation
        if (!user.AllowImpersonation) return BadRequest();

        // Simply generate a time protected query string
        var query = _cryptographyService.TimeProtectedQueryString("u", user.Id.ToString());
        return Json(new { query });
    }

    [HttpPost("/admin/users/impersonation-logon")]
    public async Task<ActionResult> ImpersonationLogon([FromBody] ImpersonationLogonModel model)
    {
        var data = model.UserId.ToString();
        const int duration = 300;
        var isValid = _cryptographyService.IsTimeProtectedQueryString(data, model.Random, model.Date, model.Hash, duration);
        if (!isValid) return Unauthorized();

        var user = await Send(new GetUserByIdRequest(model.UserId));
        if (user == null) return NotFound();
        if (!user.AllowImpersonation) return Unauthorized();

        var expiration = DateTime.UtcNow.AddHours(_securityOptions.TokenDurationInt);
        var tokenGenerator = GetService<ITokenGenerator>();
        var token = tokenGenerator.CreateToken(user.Id, user.Role.ToString(), expiration);
        var view = new LogonView(user, expiration, token);
        _userAuthenticationService.SetPrivateFileServiceCookies();

        return Json(view);
    }
}