// As this is a view, the get method of the properties may only be used by the JSON serialization
// ReSharper disable PropertyCanBeMadeInitOnly.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public;

public class DownloadRequest
{
    public int CourseId { get; set; }

    public int CourseRevisionId { get; set; }
    
    public bool IncludeMedia { get; set; }
    
    public bool Compressed { get; set; }

    public List<string> ImagesFormats { get; set; } = ["jpg", "png", "gif"];
    
    public IFileStorageService FileStorageService { get; set; }
    
    public IHttpClientFactory HttpClientFactory { get; set; }
}