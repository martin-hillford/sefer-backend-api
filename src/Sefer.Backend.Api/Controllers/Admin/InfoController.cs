using Sefer.Backend.Support.Lib.Cors;

namespace Sefer.Backend.Api.Controllers.Admin;

public class InfoController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly MailServiceOptions _mailServiceOptions = serviceProvider.GetService<IOptions<MailServiceOptions>>()?.Value;

    private readonly INotificationService _notificationService = serviceProvider.GetService<INotificationService>();

    private readonly CorsOptions _corsOptions = serviceProvider.GetService<IOptions<CorsOptions>>()?.Value;

    [HttpGet("/info/version")]
    [ProducesResponseType(typeof(VersionInfo), 200)]
    public async Task<IActionResult> GetVersionInformation()
    {
        var version = await Send(new GetVersionInfoRequest());
        version.Build = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        version.Environment = EnvVar.GetEnvironmentName();
        version.IsDevelopmentEnv = EnvVar.IsDevelopmentEnv();
        version.AdminEmail = _mailServiceOptions.AdminEmail;
        return Json(version);
    }

    [HttpGet("/info/test-email")]
    public async Task<ActionResult> SendTestEmail([FromQuery] string password, [FromQuery] string site)
    {
        if (password != _mailServiceOptions.Password) return BadRequest();

        var siteObject = await Send(new GetSiteByNameRequest(site));
        if (siteObject == null) return BadRequest();

        _notificationService.SendTestNotification(siteObject);
        return Accepted();
    }

    [HttpPost("/info/admin-test-email")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    public async Task<ActionResult> AdminSendTestEmail([FromQuery] string site)
    {
        var siteObject = await Send(new GetSiteByNameRequest(site));
        if (siteObject == null) return BadRequest();

        _notificationService.SendTestNotification(siteObject);
        return Accepted();
    }

    [HttpGet("/info/headers")]
    public IActionResult GetHeaders()
    {
        var headers = new List<KeyValuePair<string, string>>();
        foreach (var header in Request.Headers)
        {
            headers.Add(new KeyValuePair<string, string>(header.Key, header.Value));
        }
        return Json(headers);
    }

    [HttpPost("/info/cors")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCorsInfo()
    {
        var origin = Request.Headers.Origin.FirstOrDefault();
        var view = new
        {
            Origin = origin,
            Allowed = _corsOptions.ParseAllowedOrigins(),
            Raw = _corsOptions.AllowedOrigins,
            Access = _corsOptions.IsAllowed(origin),
            Referer = Request.Headers.Referer.FirstOrDefault()
        };
        return Json(view);
    }
}