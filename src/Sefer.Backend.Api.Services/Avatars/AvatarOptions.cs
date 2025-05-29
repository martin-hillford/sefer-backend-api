// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Services.Avatars;

/// <summary>
/// Class to contain the options to contact the avatar service
/// </summary>
public class AvatarOptions
{
    /// <summary>
    /// The endpoint of the service itself (without the /avatar)
    /// </summary>
    public string Service { get; set; }

    /// <summary>
    /// The ApiKey used to contact the avatar service
    /// </summary>
    public string ApiKey { get; set; }
}