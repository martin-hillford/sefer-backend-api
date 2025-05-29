// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Shared;

/// <summary>
/// The information of the user to update his profile
/// </summary>
public class ProfileInfoPostModel
{
    /// <summary>
    /// Gets / sets the name of the user
    /// </summary>
    [Required, MinLength(3), MaxLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the gender of user
    /// </summary>
    [Required, JsonConverter(typeof(JsonStringEnumConverter))]
    public Genders Gender { get; set; }

    /// <summary>
    /// Gets or sets the e-mail of the user, must be a full qualified e-mail address
    /// </summary>
    [EmailAddress, Required]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the year the user was born. Useful to determine (together with gender)
    /// the mentor to be assigned to the user
    /// </summary>
    [Range(1850, 2200), Required]
    public short YearOfBirth { get; set; }

    /// <summary>
    /// Gets/sets the password of the user
    /// </summary>
    [Required, MinLength(8)]
    public string Password { get; set; }

    /// <summary>
    /// Gets set the language code of the client, so e-mail are send in proper language
    /// </summary>
    [Required, MinLength(2)]
    public string Language { get; set; }

    /// <summary>
    /// some personal information provided by users about himself
    /// </summary>
    public string Info { get; set; }
}
