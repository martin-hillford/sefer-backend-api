// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// A post model for the logon of a user
/// </summary>
public class LogonPostModel
{
    /// <summary>
    /// The username (which is his/hers email) of the user
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password of the user
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// The two-factor code for the user
    /// </summary>
    public uint? Code { get; set; }

    /// <summary>
    /// The site where user is logging on
    /// </summary>
    public string Site { get; set; }
}
