namespace Sefer.Backend.Api.Services.Logging;

public class DatabaseLogger(string categoryName, IMediator mediator) : ILogger
{
    private static readonly List<string> InfoIgnore = ["AzureStorage", "TokenAuthenticationHandler", "Microsoft", "HttpClient"];
    
    private readonly AsyncLocal<Stack<string>> _scopes = new();
    
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.Trace; 

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
            Timestamp = DateTime.Now,
            Scope = GetCurrentScope()
        };
        mediator.Send(new AddLogRequest(logEntry));
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        _scopes.Value ??= new Stack<string>();
        _scopes.Value.Push(state?.ToString() ?? string.Empty);
        return new LoggingScope(_scopes.Value);
    }
    
    private string GetCurrentScope()
    {
        return _scopes.Value is { Count: > 0 }
            ? $"{string.Join(" => ", _scopes.Value.Reverse())}"
            : string.Empty;
    }
}
