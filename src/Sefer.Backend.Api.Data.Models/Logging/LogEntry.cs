// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Logging;

/// <summary>
/// The log entry is used for logging request that came for the user
/// </summary>
/// <inheritdoc />
public abstract class LogEntry : Entity
{
    /// <summary>
    /// The time the request was made (or as close as possible)
    /// </summary>
    public DateTime LogTime { get; set; }

    /// <summary>
    /// The Path send by the user
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// The user agent string
    /// </summary>
    public string UserAgent { get; set; }

    /// <summary>
    /// The browser token
    /// </summary>
    public string BrowserToken { get; set; }

    /// <summary>
    /// Accepted language
    /// </summary>
    public string AcceptedLanguage { get; set; }
}
