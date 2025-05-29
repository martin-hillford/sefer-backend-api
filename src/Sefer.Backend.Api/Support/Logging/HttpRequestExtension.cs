namespace Sefer.Backend.Api.Support.Logging;

/// <summary>
/// This static class extends the HttpRequest to retrieve information from the headers more easily
/// </summary>
public static class HttpRequestExtension
{
    /// <summary>
    /// Returns the path in a string version
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetPathName(this HttpRequest request)
    {
        if (request.Path.HasValue) return request.Path.Value;
        return string.Empty;
    }

    /// <summary>
    /// Returns the accepted languages of the request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetAcceptedLanguages(this HttpRequest request)
    {
        var headers = request.Headers;
        if (headers.ContainsKey("Accept-Language")) return headers["Accept-Language"];
        return string.Empty;
    }

    /// <summary>
    /// Returns the browser token embedded in the request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <remarks>Ignore the request of the user for do not track - this is analytical not tracking</remarks>
    public static string GetBrowserToken(this HttpRequest request) => GetBrowserToken(request, true);

    /// <summary>
    /// Returns the browser token embedded in the request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ignoreDoNotTrack"></param>
    /// <returns></returns>
    public static string GetBrowserToken(this HttpRequest request, bool ignoreDoNotTrack)
    {
        const string tokenKey = UserAuthenticationService.HeaderBrowserTokenKey;
        if (ignoreDoNotTrack == false && request.HasDoNotTrackRequest()) return string.Empty;
        if (request.Cookies.ContainsKey(tokenKey)) return request.Cookies[tokenKey];
        if (request.Headers.ContainsKey(tokenKey)) return request.Headers[tokenKey];
        return string.Empty;
    }

    /// <summary>
    /// Returns the user agent of the request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetUserAgent(this HttpRequest request)
    {
        var headers = request.Headers;
        if (headers.ContainsKey("User-Agent")) return headers["User-Agent"];
        return string.Empty;
    }

    /// <summary>
    /// Determines if the user has requested to do not track the user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static bool HasDoNotTrackRequest(this HttpRequest request)
    {
        var headers = request.Headers;
        return headers.ContainsKey("DNT") && headers["DNT"] == "1";
    }
}
