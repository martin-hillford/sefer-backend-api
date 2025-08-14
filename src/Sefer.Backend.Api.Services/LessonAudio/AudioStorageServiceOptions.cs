// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Services.LessonAudio;

public class AudioStorageServiceOptions
{
    public string PublicUrl { get; set; }

    public string Method { get; set; } // File or Azure (default)
    
    public string BlobSasUrl { get; set; }
    
    public string FileStoragePath { get; set; }
}