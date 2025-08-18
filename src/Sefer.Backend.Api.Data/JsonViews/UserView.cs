namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// A base view for a user
/// </summary>
/// <inheritdoc />
public class UserView : PrimitiveUserView
{
    /// <summary>
    /// Gets or sets the Role of the user
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRoles Role { get; init; }

    /// <summary>
    /// Gets or sets the gender of the user
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Genders Gender { get; init; }

    /// <summary>
    /// Gets or sets the e-mail of the user, must be a full qualified e-mail address
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Gets or sets the year the user was born. Useful to determine (together with gender)
    /// the mentor to be assigned to the user if he is a student
    /// </summary>
    public short YearOfBirth { get; init; }

    /// <summary>
    /// Gets or sets some personal information provided by users about himself
    /// </summary>
    public string Info { get; init; }

    /// <summary>
    /// Gets the SubscriptionDate of the user;
    /// that is the date the user created his account
    /// </summary>
    public DateTime SubscriptionDate { get; init; }
    
    /// <summary>
    /// Gets/sets e-mail preference of the user (no, notification, daily digest, weekly digest)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NotificationPreference NotificationPreference { get; init; }
    
    /// <summary>
    /// Indicates the user prefers spoken courses
    /// </summary>
    public bool PreferSpokenCourses { get; init; }

    /// <summary>
    /// When this is set to true, the user is allowing an admin to deliver remote support.
    /// </summary>
    public bool AllowImpersonation { get; init; }
    
    /// <summary>
    /// The primary site the user belongs to
    /// </summary>
    public string PrimarySite { get; init; }

    /// <summary>
    /// The primary site the user belongs to
    /// </summary>
    public string PrimaryRegion { get; init; }

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="user">The user of the view</param>
    public UserView(User user) : base(user)
    {
        NotificationPreference = user.NotificationPreference;
        SubscriptionDate = user.SubscriptionDate;
        Info = user.Info;
        YearOfBirth = user.YearOfBirth;
        Email = user.Email;
        Role = user.Role;
        Gender = user.Gender;
        PreferSpokenCourses = user.PreferSpokenCourses;
        AllowImpersonation = user.AllowImpersonation;
        PrimarySite = user.PrimarySite;
        PrimaryRegion = user.PrimaryRegion;
    }

    /// <summary>
    /// Constructor for json deserialization
    /// </summary>
    [JsonConstructor]
    public UserView() { }
}
