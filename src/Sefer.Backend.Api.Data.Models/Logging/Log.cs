// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Logging;

/// <summary>
/// This is log is used for application logging
/// </summary>
public class Log
{
    /// <summary>
    /// Primary key for the log event
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The time the log was made (or as close as possible)
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The severity of the log
    /// </summary>
    [MaxLength(50)]
    public string LogLevel { get; set; }

    /// <summary>
    /// The category of the log
    /// </summary>
    [MaxLength(255)]
    public string CategoryName { get; set; }

    /// <summary>
    /// The message of the log
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Message { get; set; }

    /// <summary>
    /// The message from the exception
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Exception { get; set; }

    /// <summary>
    /// The stack trace of the log
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string StackTrace { get; set; }

}
