using Microsoft.Extensions.Hosting;

namespace Sefer.Backend.Api.Services.Avatars;

public static class AvatarExtensions
{
    public static IHostApplicationBuilder AddAvatarService(this IHostApplicationBuilder builder, string section = "Avatar")
    {
        builder.Services.Configure<AvatarOptions>(builder.Configuration.GetSection(section));
        builder.Services.AddSingleton<IAvatarService, AvatarService>();
        return builder;
    }
}