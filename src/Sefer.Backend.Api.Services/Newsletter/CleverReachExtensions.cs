using Microsoft.Extensions.Hosting;
using Sefer.Backend.Api.Services.Newsletter.CleverReach;

namespace Sefer.Backend.Api.Services.Newsletter;

public static class CleverReachExtensions
{
    public static IHostApplicationBuilder AddCleverReach(this IHostApplicationBuilder builder, string section)
    {
        AddCleverReach(builder.Services, builder.Configuration, section);
        return builder;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static IServiceCollection AddCleverReach(this IServiceCollection services, IConfiguration configuration, string section)
    {
        var options = configuration.GetSection(section);
        services.Configure<CleverReachOptions>(options);
        services.AddSingleton<INewsletterService, NewsletterService>();
        return services;
    }
}