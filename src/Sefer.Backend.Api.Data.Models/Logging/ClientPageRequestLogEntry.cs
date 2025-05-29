// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Logging;

/// <summary>
/// Logging for page requests within the client
/// </summary>
/// <inheritdoc />
public class ClientPageRequestLogEntry : LogEntry
{
    /// <summary>
    /// The height of the screen
    /// </summary>
    public int ScreenHeight { get; set; }

    /// <summary>
    /// The width of the screen
    /// </summary>
    public int ScreenWidth { get; set; }

    /// <summary>
    /// The user has request not to track the user
    /// </summary>
    public bool DoNotTrack { get; set; }

    /// <summary>
    /// (Optional) the user has entered using the given ad campaign
    /// </summary>
    [MaxLength(255)]
    public string Cmp { get; set; }

    /// <summary>
    /// Defines which site the user is using
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Site { get; set; }

    /// <summary>
    /// Defines which site the user is using
    /// </summary>
    [MaxLength(50)]
    public string Region { get; set; }

    /// <summary>
    /// The ip address the user used to make the call
    /// </summary>
    [MaxLength(52)]
    public string IpAddress { get; set; }

    /// <summary>
    /// With this id this request and the users location can be correlated
    /// </summary>
    public Guid? GeoIPInfoId { get; set; }

    /// <summary>
    /// The browser class of the request
    /// </summary>
    [MaxLength(30)]
    public string BrowserClass { get; set; }

    /// <summary>
    /// The browser class of the request
    /// </summary>
    [MaxLength(30)]
    public string OperatingSystem { get; set; }

    /// <summary>
    /// Holds if request was performed by a bot
    /// </summary>
    public bool IsBot { get; set; }
}
