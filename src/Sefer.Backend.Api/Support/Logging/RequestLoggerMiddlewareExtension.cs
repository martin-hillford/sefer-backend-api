using Microsoft.AspNetCore.Builder;

namespace Sefer.Backend.Api.Support.Logging;

/// <summary>
/// RequestLoggerMiddlewareExtension provides an extension of the middleware that takes care logging incoming requests
/// </summary>
public static class RequestLoggerMiddlewareExtension
{
    /// <summary>
    /// Add middleware that will deal with logging requests op the api
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseRequestLogger(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggerHandler>();
    }
}
