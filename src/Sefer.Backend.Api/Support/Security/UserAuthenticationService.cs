namespace Sefer.Backend.Api.Support.Security;

public class UserAuthenticationService(IServiceProvider serviceProvider) : IUserAuthenticationService
{
    public const string HeaderBrowserTokenKey = "X-BrowserToken";

    private const string CookieFileAccessDate = "FileAccessDate";

    private const string CookieFileAccessRandom = "FileAccessRandom";

    private const string CookieFileAccessHash = "FileAccessHash";

    private readonly IPasswordService _passwordService = serviceProvider.GetService<IPasswordService>();

    private readonly ICryptographyService _cryptographyService = serviceProvider.GetService<ICryptographyService>();

    private readonly SecurityOptions _securityOptions = serviceProvider.GetService<IOptions<SecurityOptions>>().Value;

    private readonly IHttpContextAccessor _httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

    private readonly ITokenGenerator _tokenGenerator = serviceProvider.GetService<ITokenGenerator>();

    public int? UserId => GetUserId(_httpContextAccessor);

    public UserRoles? UserRole => GetUserRole(_httpContextAccessor);

    public bool IsAuthenticated => UserId != null;

    public async Task<SignOnResult> SignOn(string email, string password)
    {
        var mediator = serviceProvider.GetService<IMediator>();
        if (mediator == null) return SignOnResult.IncorrectSignIn;

        var user = await mediator.Send(new GetUserByEmailRequest(email));
        if (user == null) return SignOnResult.IncorrectSignIn;
        if (user.Blocked) return SignOnResult.Blocked;
        if (user.Approved == false) return SignOnResult.Unapproved;
        var valid = _passwordService.IsValidPassword(user, password);

        return valid ? SignOnResult.Success : SignOnResult.IncorrectSignIn;
    }

    public bool IsAuthorized(UserRoles role) => UserRole == role;

    public void SetPrivateFileServiceCookies()
    {
        if (_securityOptions.UseFileServiceCookieBool == false) return;

        var culture = new CultureInfo("en-US");
        var random = _cryptographyService.UrlHash(_cryptographyService.Salt());
        var date = DateTime.UtcNow.ToString("O", culture);
        var hash = _cryptographyService.UrlHash(date, random);
        var options = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(_securityOptions.TokenDurationInt),
            Secure = _securityOptions.CookieSecureBool,
            Path = _securityOptions.CookiePath
        };

        if (_httpContextAccessor?.HttpContext == null) return;
        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieFileAccessDate, date, options);
        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieFileAccessRandom, random, options);
        _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieFileAccessHash, hash, options);
    }

    public void SetUserCookies(User user, bool isAppLogin = false)
    {
        var expiration = isAppLogin
            ? DateTime.UtcNow.AddYears(2000)
            : DateTime.UtcNow.AddHours(_securityOptions.TokenDurationInt);

        var context = _httpContextAccessor.HttpContext;
        if(context == null) return;
        var cookieAuth = new AuthCookieService(_tokenGenerator, context, _securityOptions);
        var token = _tokenGenerator.CreateToken(user.Id, user.Role.ToString(), expiration);
        cookieAuth.AppendAuthCookie(user.Id, token, expiration);
    }

    public bool IsFileAuthenticated()
    {
        if (_httpContextAccessor?.HttpContext == null) return false;

        var request = _httpContextAccessor.HttpContext.Request;
        if (request.Cookies.ContainsKey(CookieFileAccessDate) == false) return false;
        if (request.Cookies.ContainsKey(CookieFileAccessRandom) == false) return false;
        if (request.Cookies.ContainsKey(CookieFileAccessHash) == false) return false;

        // check the date
        var culture = new CultureInfo("en-US");
        var parsed = DateTime.TryParseExact(request.Cookies[CookieFileAccessDate], "O", culture, DateTimeStyles.None, out DateTime date);
        if (parsed == false || date.AddHours(_securityOptions.TokenDurationInt) < DateTime.UtcNow) return false;

        // check the hash
        var random = request.Cookies[CookieFileAccessRandom];
        var hash = request.Cookies[CookieFileAccessHash];
        var valid = _cryptographyService.IsValidUrlHash(hash, request.Cookies[CookieFileAccessDate], random);

        // done checking
        return valid;
    }

    private static int? GetUserId(IHttpContextAccessor contextAccessor)
    {
        var value = contextAccessor?.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (value == null) return null;

        if (!int.TryParse(value, out var userId)) return null;
        return userId;
    }

    private static UserRoles? GetUserRole(IHttpContextAccessor contextAccessor)
    {
        var value = contextAccessor?.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (value == null) return null;

        if (!Enum.TryParse(value, out UserRoles userRole)) return null;
        return userRole;
    }
}
