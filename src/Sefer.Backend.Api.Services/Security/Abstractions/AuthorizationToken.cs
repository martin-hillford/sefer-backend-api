using Sefer.Backend.Api.Data.Models.Constants;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Sefer.Backend.Api.Services.Security.Abstractions;

/// <summary>
/// The AuthorizationToken, used for authorization.
/// Is not readable for user in unencrypted form
/// </summary>
public class AuthorizationToken
{
    /// <summary>
    /// A sessionId for the logon
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// The id of user represent by this token
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The Role of the user
    /// </summary>
    public UserRoles UserRole { get; set; }

    /// <summary>
    /// The time till this token is valid
    /// </summary>
    public DateTime ExpirationDateTime { get; set; }

    /// <summary>
    /// Returns a unique id based on the token
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public string UniqueId => SessionId + UserId.ToString() + UserRole + ExpirationDateTime.ToString("yyyyMMddhhmm");
}