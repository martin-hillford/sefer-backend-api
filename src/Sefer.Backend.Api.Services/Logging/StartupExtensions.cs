namespace Sefer.Backend.Api.Services.Logging;

public static class StartupExtensions
{
    public static IServiceCollection AddDatabaseLogging(this IServiceCollection services)
        => services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();
}