// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Services.FileStorage.FileSystemStorage;

/// <summary>
/// Options for the FileSystemStorageService
/// </summary>
public class FileSystemStorageServiceOptions
{
    /// <summary>
    /// A path where public files are stored
    /// </summary>
    public string PublicPath { get; set; }

    /// <summary>
    /// A path where private files are stored
    /// </summary>
    public string PrivatePath { get; set; }

    /// <summary>
    /// A file storage will always handle internally, this provides the base url
    /// </summary>
    public string Endpoint { get; set; }
}