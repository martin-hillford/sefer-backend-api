// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// Activation of a user account from the app
/// </summary>
public class AppActivationPostModel
{
    /// <summary>
    /// The code that the user has received via e-mail
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// The e-mail address of the user to identify the user
    /// </summary>
    public string Email { get; set; }
}
