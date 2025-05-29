namespace Sefer.Backend.Api.Data.Handlers.Helpers;

public class HelperClass
{
    protected readonly DataContext Context;

    protected readonly IServiceProvider ServiceProvider;

    protected HelperClass(DataContext context)
    {
        Context = context;
    }

    protected HelperClass(IServiceProvider serviceProvider, DataContext context)
    {
        Context = context;
        ServiceProvider = serviceProvider;
    }
}