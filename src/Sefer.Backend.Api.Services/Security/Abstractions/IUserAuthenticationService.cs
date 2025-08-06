using Sefer.Backend.Api.Data.Models.Constants;
// ReSharper disable UnusedMember.Global

namespace Sefer.Backend.Api.Services.Security.Abstractions;

/// <summary>
/// The IUserAuthenticationService defines a UserAuthenticationService which is capable of
/// the authentication of users, and if in the current state, a user is authenticated and to retrieve that user
/// </summary>
public interface IUserAuthenticationService
{
    /// <summary>
    /// Gets the current user id, if authenticated, else null is returned
    /// </summary>
    int? UserId { get; }

    /// <summary>
    /// get the current role of the user if authenticated else null is returned
    /// </summary>
    UserRoles? UserRole { get; }

    /// <summary>
    /// Gets if a user is authenticated
    /// </summary>
    /// <returns>When a user is authenticated, true returned, in all other cases false</returns>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Signs on a user given his e-mail and password
    /// </summary>
    /// <param name="email">The e-mail provided by the user</param>
    /// <param name="password">The password provided by the user</param>
    /// <returns>Returns the result of the SingOn. If Success is returned, IsAuthenticated should return in requests to come.</returns>
    Task<SignOnResult> SignOn(string email, string password);

    /// <summary>
    /// Check if the current user is authorized for the provided role
    /// </summary>
    /// <param name="role">The role to check for if the current user (is any) is authorized to</param>
    /// <returns>True when the user is authorized else false</returns>
    bool IsAuthorized(UserRoles role);

    /// <summary>
    /// This method will set the cookies for service private files to the logged on user
    /// </summary>
    void SetPrivateFileServiceCookies();

    /// <summary>
    /// This will check if the user is authenticated for viewing private files
    /// </summary>
    /// <returns></returns>
    bool IsFileAuthenticated();

    /// <summary>
    /// This method will set the correct cookies.
    /// </summary>
    /// <remarks>
    /// One day 
    /// </remarks>
    void SetUserCookies(User user, bool isAppLogin = false);
}
