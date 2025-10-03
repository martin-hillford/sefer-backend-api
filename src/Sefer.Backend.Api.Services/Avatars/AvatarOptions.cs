// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Services.Avatars;

/// <summary>
/// Class to contain the options to contact the avatar service
/// </summary>
public class AvatarOptions
{
    /// <summary>
    /// The ApiKey used to contact the avatar service
    /// </summary>
    public string HashKey { get; set; }
    
    /// <summary>
    /// The url used for the service
    /// </summary>
    public string Service { get; set;  }
    
    public string Store { get; set; }
    
    public bool UseBlob { get; set; }
}