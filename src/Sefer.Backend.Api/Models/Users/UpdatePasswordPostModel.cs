// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Users;

/// <summary>
/// Used to update the profile password of a logged on user
/// </summary>
public class UpdatePasswordPostModel
{
    /// <summary>
    /// The language code of the mail to send to the user
    /// </summary>
    [Required]
    public string Language { get; set; }

    /// <summary>
    /// The old password of the user
    /// </summary>
    [Required]
    public string OldPassword { get; set; }

    /// <summary>
    /// The new password of the user
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    /// <summary>
    /// A confirmation of the new password of the user
    /// </summary>
    [Required]
    [MinLength(8)]
    public string ConfirmNewPassword { get; set; }
}
