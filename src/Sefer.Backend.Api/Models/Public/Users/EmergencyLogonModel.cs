// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// Post model for emergency logon when users don't have access to their two-factor auth device
/// </summary>
public class EmergencyLogonModel
{
    /// <summary>
    /// The username of the user
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password of the user
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// The backup key of the user
    /// </summary>
    public string BackupKey { get; set; }

    /// <summary>
    /// The language of the user
    /// </summary>
    public string Language { get; set; }
}