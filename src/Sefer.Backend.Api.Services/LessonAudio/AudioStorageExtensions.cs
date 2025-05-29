using Microsoft.Extensions.Hosting;

namespace Sefer.Backend.Api.Services.LessonAudio;

public static class AudioStorageExtensions
{
    public static IHostApplicationBuilder AddAudioStorageService(this IHostApplicationBuilder builder, string section = "AudioStorage")
    {
        builder.Services.Configure<AudioStorageServiceOptions>(builder.Configuration.GetSection(section));
        builder.Services.AddSingleton<IAudioStorageService, AudioStorageService>();
        return builder;
    }
}