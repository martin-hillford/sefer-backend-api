namespace Sefer.Backend.Api.Support.Logging;

public class RequestLoggerHandlerParams(
    HttpRequest httpRequest,
    IServiceProvider serviceProvider,
    DateTime requestTime,
    long processingTime)
{
    public readonly IServiceProvider ServiceProvider = serviceProvider;

    public readonly ApiRequestLogEntry ApiRequestLogEntry = new()
    {
        LogTime = requestTime,
        ProcessingTime = processingTime,
        Method = httpRequest.Method,
        Path = httpRequest.GetPathName(),
        AcceptedLanguage = httpRequest.GetAcceptedLanguages(),
        BrowserToken = httpRequest.GetBrowserToken(),
        UserAgent = httpRequest.GetUserAgent(),
        DoNotTrack = httpRequest.HasDoNotTrackRequest()
    };
}