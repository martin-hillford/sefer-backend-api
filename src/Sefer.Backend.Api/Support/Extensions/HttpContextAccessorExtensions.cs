using Sefer.Backend.Api.Models.Public.Home;
using Sefer.Backend.GeoIP.Lib;
// ReSharper disable EmptyGeneralCatchClause

namespace Sefer.Backend.Api.Support.Extensions;

public static class HttpContextAccessorExtensions
{
    private static readonly string[] Bots = ["bot", "crawler", "Wappalyzer", "google-adwords", "woorankreview"];

    public static async Task<ClientPageRequestLogEntry> GetLogEntry(this IHttpContextAccessor contextAccessor, PageLogPostModel pageLog, IGeoIPService geoIPService)
    {
        if (pageLog == null) return null;

        // get the request object
        var request = contextAccessor?.HttpContext?.Request;
        if (request == null) return null;

        // get the UTC date of the log and don't log it, if it's too much off
        var now = DateTime.UtcNow;
        var date = DateTimeOffset.FromUnixTimeMilliseconds(pageLog.Time).UtcDateTime;
        if (date < now.AddHours(-1) || date > now.AddHours(1)) return null;

        // Based on the provided information of the user and request log the page request
        var ipAddress = request.GetClientIpAddress();
        var geoInfo = await geoIPService.GetInfo(ipAddress);
        var userAgent = request.GetUserAgent();

        return new ClientPageRequestLogEntry
        {
            AcceptedLanguage = request.GetAcceptedLanguages(),
            BrowserToken = request.GetBrowserToken(),
            LogTime = date,
            Path = pageLog.Page,
            UserAgent = userAgent,
            ScreenHeight = pageLog.ScreenHeight,
            ScreenWidth = pageLog.ScreenWidth,
            DoNotTrack = request.HasDoNotTrackRequest(),
            Cmp = pageLog.Cmp,
            Site = pageLog.Site,
            IpAddress = ipAddress,
            GeoIPInfoId = geoInfo?.Id,
            BrowserClass = GetBrowserClass(userAgent),
            OperatingSystem = GetOperatingSystem(userAgent),
            IsBot = IsBot(userAgent),
            Region = pageLog.Region,
        };
    }

    public static string GetClientIpAddress(this IHttpContextAccessor contextAccessor) => contextAccessor?.HttpContext?.Request.GetClientIpAddress();

    public static string GetClientIpAddress(this HttpRequest request)
    {
        var forwardedIp = GetAzureForwardedIp(request);
        if (!string.IsNullOrEmpty(forwardedIp)) return forwardedIp;

        var realIp = request?.Headers.Get("X-Real-IP");
        return string.IsNullOrEmpty(realIp)
            ? request?.HttpContext.Connection.RemoteIpAddress?.ToString()
            : realIp;
    }

    private static string Get(this IHeaderDictionary dictionary, string headerName)
    {
        try { if (dictionary.TryGetValue(headerName, out var value)) return value; }
        catch (Exception) { }
        return null;
    }

    private static string GetBrowserClass(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return "Unknown";

        try
        {
            if (userAgent.Contains("Firefox")) return "Firefox";
            if (userAgent.Contains("MSIE", StringComparison.OrdinalIgnoreCase)) return "Internet Explorer";
            if (userAgent.Contains("SamsungBrowser")) return "Samsung Browser";
            if (userAgent.Contains("Edg")) return "Microsoft Edge";
            if (userAgent.Contains("Chrome")) return "Chrome";
            if (userAgent.Contains("Safari")) return "Safari";
            if (userAgent.Contains("OP")) return "Opera";
        }
        catch (Exception) { }
        return "Unknown";
    }

    private static bool IsBot(string userAgent)
    {
        try
        {
            return !string.IsNullOrEmpty(userAgent) && Bots.Any(userAgent.Contains);
        }
        catch (Exception) { return false; }
    }

    private static string GetOperatingSystem(string userAgent)
    {
        if (string.IsNullOrEmpty(userAgent)) return "Unknown";
        try
        {
            if (userAgent.Contains("iPhone")) return "iPhone";
            if (userAgent.Contains("iPad")) return "iPad";
            if (userAgent.Contains("CrOS")) return "Chrome OS";
            if (userAgent.Contains("Windows")) return "Windows";
            if (userAgent.Contains("Tizen")) return "Tizen";
            if (userAgent.Contains("Android")) return "Android";
            if (userAgent.Contains("Mac OS")) return "Mac";
            if (userAgent.Contains("MacOS")) return "Mac";
            if (userAgent.Contains("Ubuntu")) return "Linux";
            if (userAgent.Contains("Fedora")) return "Linux";
            if (userAgent.Contains("Linux")) return "Linux";
        }
        catch (Exception) { }
        return "Unknown";
    }

    private static string GetAzureForwardedIp(HttpRequest request)
    {
        // The problems with azure is their linux apps are running behind
        // a proxy that will forward the request. This need to dealt with.
        var forwardedIp =
            request?.Headers.Get("x-Forwarded-For") ??
            request?.Headers.Get("X-Forwarded-For");

        // Azure also includes the port number for the forward,
        // this method is not interested in that.
        if (string.IsNullOrEmpty(forwardedIp)) return null;
        return !forwardedIp.Contains(':') ? forwardedIp : forwardedIp.Split(':')[0].Trim();
    }
}

