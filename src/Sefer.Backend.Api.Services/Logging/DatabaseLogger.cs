namespace Sefer.Backend.Api.Services.Logging;

public class DatabaseLogger(string categoryName, IMediator mediator) : ILogger
{
    private static readonly List<string> InfoIgnore = ["AzureStorage", "TokenAuthenticationHandler", "Microsoft", "HttpClient"];

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (categoryName.Contains("EntityFramework")) return;
        if (InfoIgnore.Any(categoryName.Contains) && logLevel == LogLevel.Information) return;
        if (logLevel != LogLevel.None) RecordMsg(logLevel, eventId, state, exception, formatter);
    }

    private void RecordMsg<TState>(LogLevel logLevel, EventId _, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var logEntry = new Log
        {
            LogLevel = logLevel.ToString(),
            CategoryName = categoryName,
            Message = formatter(state, exception),
            StackTrace = exception?.StackTrace,
            Exception = exception?.Message,
            Timestamp = DateTime.Now
        };
        mediator.Send(new AddLogRequest(logEntry));
    }

    public IDisposable BeginScope<TState>(TState state) => new NoopDisposable();
    
    private sealed class NoopDisposable : IDisposable
    {
        public void Dispose() { }
    }
}