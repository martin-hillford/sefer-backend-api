// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// Container for requesting a password forgot e-mail
/// </summary>
public class PasswordForgotPostModel
{
    /// <summary>
    /// The email address of the user to send a message for
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// The language code of the client, so e-mail are send in proper language
    /// </summary>
    [Required]
    [MinLength(2)]
    public string Language { get; set; }
}
