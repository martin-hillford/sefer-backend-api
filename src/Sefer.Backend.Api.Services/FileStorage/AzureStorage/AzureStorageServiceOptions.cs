// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Services.FileStorage.AzureStorage;

/// <summary>
/// Options for the configuration of the azure storage services
/// </summary>
public class AzureStorageServiceOptions
{
    /// <summary>
    /// The base url for public files (to be used to create url's that can be used in browsers)
    /// </summary>
    public string PublicUrl { get; set; }

    /// <summary>
    /// Access tokens with read/write permissions on the public space
    /// </summary>
    public string PublicSas { get; set; }

    /// <summary>
    /// the base url for private files (to be used to create url's that can be used in browsers)
    /// </summary>
    public string PrivateUrl { get; set; }

    /// <summary>
    /// Access tokens with read/write permissions on the private space
    /// </summary>
    public string PrivateSas { get; set; }
}
