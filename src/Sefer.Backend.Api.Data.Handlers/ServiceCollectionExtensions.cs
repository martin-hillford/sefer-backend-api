namespace Sefer.Backend.Api.Data.Handlers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediation(this IServiceCollection services)
    {
        var assembly = typeof(ServiceCollectionExtensions).Assembly;
        services.AddMediator(assembly);

        var type = typeof(ICustomValidationService);
        assembly
            .GetTypes()
            .Where(t => t.GetInterfaces().Contains(type) && !t.IsAbstract && t.IsClass)
            .ToListThenForEach(services.Register);

        return services;
    }

    private static void Register(this IServiceCollection services, Type implementationType)
    {
        var serviceType = implementationType
            .GetInterfaces()
            .FirstOrDefault(t =>
                t.IsGenericType &&
                t.AssemblyQualifiedName?.Contains("Sefer.Backend.Api.Data.Handlers.Validation.ICustomValidationService") == true
            );

        if (serviceType != null) services.AddSingleton(serviceType, implementationType);
    }
}