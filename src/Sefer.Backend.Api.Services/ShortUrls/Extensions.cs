using Microsoft.Extensions.Hosting;

namespace Sefer.Backend.Api.Services.ShortUrls;

public static class Extensions
{
    public static IHostApplicationBuilder AddShortUrlService(this IHostApplicationBuilder builder, string section = "ShortUrlService")
    {
        AddShortUrlService(builder.Services, builder.Configuration, section);
        return builder;
    }

    private static void AddShortUrlService(this IServiceCollection services, IConfiguration configuration, string section = "ShortUrlService")
    {
        var options = configuration.GetSection(section);
        services.Configure<ShortUrlServiceOptions>(options);
        services.AddSingleton<IShortUrlService, ShortUrlService>();
    }
}