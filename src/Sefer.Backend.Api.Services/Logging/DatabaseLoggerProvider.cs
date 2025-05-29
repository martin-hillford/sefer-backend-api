namespace Sefer.Backend.Api.Services.Logging;

public class DatabaseLoggerProvider(IMediator mediator) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName)
    {
        return new DatabaseLogger(categoryName, mediator);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}