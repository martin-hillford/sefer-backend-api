// This is a model, so the constructor or some setter may not be used explicitly.
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.Text.Json;
using Sefer.Backend.Api.Shared.Validation;

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
    /// Gets or sets the gender of the user
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
    /// Gets set the language code of the client, so e-mails are sent in the proper language
    /// </summary>
    [Required, MinLength(2)]
    public string Language { get; set; }

    /// <summary>
    /// some personal information provided by users about himself
    /// </summary>
    public string Info { get; set; }
    

    /// <summary>
    /// In several sefer applications it is desirable to store more information
    /// on the user. Like country, location, but this is not needed for all
    /// applications, so this is build flexibility
    /// </summary>
    [JsonDictionary("UserAdditionalInfo",4000)]
    public Dictionary<string, JsonElement> AdditionalInfo { get; set; }
}
