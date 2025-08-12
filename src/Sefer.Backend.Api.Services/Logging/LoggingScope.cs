namespace Sefer.Backend.Api.Services.Logging;

public class LoggingScope(Stack<string> stack) : IDisposable
{
    public void Dispose()
    {
        if (stack.Count > 0) stack.Pop();
    }
}