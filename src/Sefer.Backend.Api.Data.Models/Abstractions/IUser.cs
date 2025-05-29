namespace Sefer.Backend.Api.Data.Models.Abstractions;

/// <summary>
/// Defines an interface for an IUser that will be used in conjunction with a IPasswordService 
/// </summary>
public interface IUser
{
    /// <summary>
    /// Gets/sets the password of the user (encrypted)
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// Gets/sets the salt used for password of the user (used for password encryption) 
    /// </summary>
    string PasswordSalt { get; set; }
}