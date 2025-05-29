using Sefer.Backend.Api.Data.Models.Abstractions;

namespace Sefer.Backend.Api.Services.Security.Abstractions;

/// <summary>
/// A Password service helps in setting and updating passwords
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// This method updates the password of the given user
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    void UpdatePassword(IUser user, string password);

    /// <summary>
    /// This method validates if the given password is equal to the users
    /// </summary>
    /// <param name="user"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    bool IsValidPassword(IUser user, string password);
}