using Microsoft.Extensions.Hosting;

namespace Sefer.Backend.Api.Services.LessonAudio;

public static class AudioStorageExtensions
{
    public static IHostApplicationBuilder AddAudioStorageService(this IHostApplicationBuilder builder, string section = "AudioStorage")
    {
        var config = builder.Configuration.GetSection(section);
        if(config["Method"] == "File") builder.Services.AddSingleton<IAudioStorageService, AudioFileStorageService>();
        else builder.Services.AddSingleton<IAudioStorageService, AudioAzureStorageService>();
        builder.Services.Configure<AudioStorageServiceOptions>(config);
        return builder;
    }
}