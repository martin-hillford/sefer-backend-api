namespace Sefer.Backend.Api.Data.Handlers.Test;

public class DataContextExceptionProvider(Exception exception) : IDataContextProvider
{
    // ReSharper disable once UnusedMember.Global
    public DataContextExceptionProvider() : this(new Exception()) {  }

    public DataContext GetContext()
    {
        throw exception;
    }
}