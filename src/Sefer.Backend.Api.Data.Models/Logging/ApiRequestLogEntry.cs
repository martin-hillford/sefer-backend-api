// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Logging;

/// <summary>
/// A model for the ApiRequestLogEntry
/// </summary>
/// <inheritdoc />
public class ApiRequestLogEntry: LogEntry
{
    /// <summary>
    /// The number of milliseconds the request took to process
    /// </summary>
    public long ProcessingTime { get; set; }

    /// <summary>
    /// The method that was used withing the request
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Method { get; set; }

    /// <summary>
    /// The user has request not to track the user
    /// </summary>
    public bool DoNotTrack { get; set; }
}