// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// Very simple post model with an e-mail to validate for it's uniqueness
/// </summary>
public class IsEmailUniquePostModel
{
    /// <summary>
    /// The e-mail to use for validation
    /// </summary>
    public string Email { get; set; }
}
