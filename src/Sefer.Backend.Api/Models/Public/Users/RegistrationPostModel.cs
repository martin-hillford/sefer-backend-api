// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json;
using Sefer.Backend.Api.Shared.Validation;

namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// A model to post a new registration
/// </summary>
public class RegistrationPostModel
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
    [Required, EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the year the user was born. Useful to determine (together with gender)
    /// the mentor to be assigned to the user if he is a student
    /// </summary>
    [Required, Range(1850, 2200)]
    public short YearOfBirth { get; set; }

    /// <summary>
    /// Gets/sets the password of the user
    /// </summary>
    [Required, MinLength(8), MaxLength(int.MaxValue)]
    public string Password { get; set; }

    /// <summary>
    /// Gets set the language code of the client, so e-mails are sent in the proper language
    /// </summary>
    [Required, MinLength(2), MaxLength(3)]
    public string Language { get; set; }

    /// <summary>
    /// The identifier (hostname) of the site that user is in
    /// </summary>
    public string Site { get; set; }

    /// <summary>
    /// The region the user is subscribing to
    /// </summary>
    public string Region { get; set; }

    /// <summary>
    /// Holds the mode (app or web)
    /// </summary>
    public string Mode { get; set; }

    /// <summary>
    /// Holds if the registration was done in app or on the web
    /// </summary>
    public bool IsInAppRegistration => Mode == "app";

    /// <summary>
    /// In several sefer applications it is desirable to store more information
    /// on the user. Like country, location, but this is not needed for all
    /// applications, so this is build flexibility
    /// </summary>
    [JsonDictionary("UserAdditionalInfo",4000)]
    public Dictionary<string, JsonElement> AdditionalInfo { get; set; }
    
    /// <summary>
    /// If this is a registration through an invitation of a mentor,
    /// this contains the information of that invitation
    /// </summary>
    public InvitationPostModel Invitation { get; set; }

    public User Create(ISite site, IRegion region, bool approved = false)
    {
        return new User
        {
            Active = false,
            Approved = approved,
            Blocked = false,
            Email = Email,
            Gender = Gender,
            Name = Name,
            Role = UserRoles.User,
            YearOfBirth = YearOfBirth,
            SubscriptionDate = DateTime.UtcNow,
            NotificationPreference = NotificationPreference.Direct,
            PreferredInterfaceLanguage = Language,
            PrimarySite = site.Hostname,
            PrimaryRegion = region.Id,
            AdditionalInfo = AdditionalInfo
        };
    }
}