namespace Sefer.Backend.Api.Support.Logging;

public class RequestLoggerHandler(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, IServiceProvider serviceProvider)
    {
        // Check if this in an options request
        var isOptionsRequest = context.Request.Method.ToLower() == "options";

        // Start watching the clock
        var watch = Stopwatch.StartNew();
        var requestTime = DateTime.UtcNow;

        // Add processing
        context.Response.OnStarting(OnStarting, context);

        // continue the middleware
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var logger = serviceProvider.GetService<ILogger<RequestLoggerHandler>>();
            logger?.LogCritical(exception, "Unhandled exception occurred.");
        }

        // Options requests are not logged
        if (isOptionsRequest) return;
        
        // take the processing time add it to the header
        watch.Stop();
        var logParams = new RequestLoggerHandlerParams(context.Request, serviceProvider, requestTime, watch.ElapsedMilliseconds);

        // And schedule the log for being written
        LogUserRequest(logParams);

        return;

        Task OnStarting(object state)
        {
            var httpContext = (HttpContext)state;
            httpContext.Response.Headers["X-ProcessingTime"] = $"{watch.ElapsedMilliseconds}";
            return Task.CompletedTask;
        }
    }

    private static void LogUserRequest(object state)
    {
        if (state is not RequestLoggerHandlerParams logParams || logParams.ApiRequestLogEntry == null) return;
        var entry = logParams.ApiRequestLogEntry;

        // ignore empty logs
        var props = new List<string> { entry.AcceptedLanguage, entry.BrowserToken, entry.AcceptedLanguage, entry.Path };
        if (props.All(string.IsNullOrEmpty)) return;
        var request = new AddApiRequestLogEntryRequest(entry);
        var mediator = logParams.ServiceProvider.GetService<IMediator>();

        // By running the task out of context, the request can finish before the logging is complete
        Task.Run(() => { mediator.Send(request); });
    }
}
