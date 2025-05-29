// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Student.Profile;

/// <summary>
/// Container for requesting a account delete e-mail
/// </summary>
public class AccountDeletePostModel
{
    /// <summary>
    /// The language code of the client, so e-mail are send in proper language
    /// </summary>
    [Required, MinLength(2)]
    public string Language { get; set; }
}