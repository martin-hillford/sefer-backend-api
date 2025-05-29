// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Services.LessonAudio;

public class AudioStorageServiceOptions
{
    public string PublicUrl { get; set; }

    public string BlobSasUrl { get; set; }
}