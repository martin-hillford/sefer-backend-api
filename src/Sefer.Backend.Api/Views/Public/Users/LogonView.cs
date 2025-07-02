// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Users;

/// <summary>
/// View to returns logon information to client
/// </summary>
/// <remarks>
/// Creates a new view
/// </remarks>
/// <param name="user">The user that just logged on</param>
/// <param name="token">The token to user for logon</param>
/// <param name="settings">The settings to the user</param>
/// <param name="expiration">The expiration date of the token</param>
public sealed class LogonView(User user, List<UserSetting> settings, DateTime expiration, string token)
{
    /// <summary>
    /// The user that just logged on
    /// </summary>
    public readonly UserWithSettingsView User = new(user, settings);

    /// <summary>
    /// The token to user for logon
    /// </summary>
    public readonly string Token = token;

    /// <summary>
    /// The primary region this user belongs to
    /// </summary>
    public readonly string Region = user.PrimaryRegion;

    /// <summary>
    /// The expiration date of the token
    /// </summary>
    public readonly long Expires = ((DateTimeOffset)expiration.ToUniversalTime()).ToUnixTimeSeconds();
}
