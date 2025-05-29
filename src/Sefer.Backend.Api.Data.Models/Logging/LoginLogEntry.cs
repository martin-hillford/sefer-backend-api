// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Logging;

/// <summary>
/// This class deals with login logging
/// </summary>
/// <inheritdoc />
public class LoginLogEntry : LogEntry
{
    /// <summary>
    /// The username used for the login
    /// </summary>
    [MaxLength(450)]
    public string Username { get; set; }

    /// <summary>
    /// The result of the login request
    /// </summary>
    public SignOnResult Result { get; set; }

    /// <summary>
    /// The IP address of the request origin
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string IpAddress { get; set; }

    /// <summary>
    /// For logon request we will save the user id
    /// </summary>
    public int? UserId { get; set; }
}