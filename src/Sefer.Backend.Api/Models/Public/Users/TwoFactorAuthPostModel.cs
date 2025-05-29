// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// This model deal with the code in the two-factor auth
/// </summary>
public class TwoFactorAuthPostModel
{
    /// <summary>
    /// The code to authenticate the user with
    /// </summary>
    public uint Code { get; set; }

    /// <summary>
    /// The language of the user
    /// </summary>
    public string Language { get; set; }
}