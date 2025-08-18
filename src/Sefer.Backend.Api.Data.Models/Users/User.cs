// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
using System.Text.Json;
using Sefer.Backend.Api.Shared.Validation;

namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// The User class deals with all types of users.
/// </summary>
/// <inheritdoc cref="Entity" />
/// <inheritdoc cref="IUser" />
public class User : Entity, IUser
{
    /// <summary>
    /// Gets / sets the name of the user
    /// </summary>
    [Required, MinLength(3), MaxLength(255)]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the Role of the user
    /// </summary>
    public UserRoles Role { get; set; }
    
    /// <summary>
    /// Gets or sets the gender of the user
    /// </summary>
    [Required]
    public Genders Gender { get; set; }

    /// <summary>
    /// Gets or sets the e-mail of the user, must be a full qualified e-mail address
    /// </summary>
    [EmailAddress, Required, MaxLength(450)]
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the year the user was born. Useful to determine (together with gender)
    /// the mentor to be assigned to the user if he is a student
    /// </summary>
    [Required, Range(1850, 2200)]
    public short YearOfBirth { get; set; }

    /// <summary>
    /// Gets or sets some personal information provided by users about himself
    /// </summary>
    [MaxLength(int.MaxValue)]
    public string Info { get; set; }

    /// <summary>
    /// Gets/sets the password of the user (encrypted)
    /// </summary>
    /// <inheritdoc cref="IUser.Password"/>
    [MaxLength(int.MaxValue)]
    public string Password { get; set; }

    /// <summary>
    /// Gets/sets the salt used for password of the user (used for password encryption)
    /// </summary>
    /// <inheritdoc cref="IUser.PasswordSalt"/>
    [MaxLength(int.MaxValue)]
    public string PasswordSalt { get; set; }

    /// <summary>
    /// Gets the SubscriptionDate of the user;
    /// that is the date the user created his account
    /// </summary>
    [InsertOnly]
    public DateTime SubscriptionDate { get; set; }

    /// <summary>
    /// Gets/sets if the user is Active
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets if the user is approved, that is he followed the activation link in the e-mail
    /// </summary>
    public bool Approved { get; set; }

    /// <summary>
    /// Gets if the user is blocked.
    /// </summary>
    public bool Blocked { get; set; }
    
    /// <summary>
    /// Gets/sets e-mail preference of the user (no, notification, daily digest, weekly digest)
    /// </summary>
    public NotificationPreference NotificationPreference { get; set; }
    
    /// <summary>
    /// Holds if the enrollment is imported from the old system
    /// </summary>
    /// <value></value>
    public bool Imported { get; set; }

    /// <summary>
    /// Holds the PreferredInterfaceLanguage for the given user. Default is Dutch
    /// </summary>
    /// <remarks>The preferred language of the user is also used when sending e-mail</remarks>
    [MaxLength(3)]
    public string PreferredInterfaceLanguage { get; set; } = "nl";

    /// <summary>
    /// Holds if the two-factor authentication is enabled
    /// </summary>
    public bool TwoFactorAuthEnabled { get; set; }

    /// <summary>
    /// Holds the secret key used for two-factor authentication
    /// </summary>
    [JsonIgnore, MaxLength(64)]
    public string TwoFactorAuthSecretKey { get; set; }

    /// <summary>
    /// Indicates the user prefers spoken courses
    /// </summary>
    /// <remarks>This is used in the course recommender, so it should be part of the admin</remarks>
    public bool PreferSpokenCourses { get; set; }

    /// <summary>
    /// When this is set to true, the user is allowing an admin to deliver remote support.
    /// </summary>
    public bool AllowImpersonation { get; set; }

    /// <summary>
    /// In several sefer applications it is desirable to store more information
    /// on the user. Like country, location, but this is not needed for all
    /// applications, so this is build flexibility
    /// </summary>
    [JsonDictionary("UserAdditionalInfo",4000)]
    public Dictionary<string, JsonElement> AdditionalInfo { get; set; }
    
    /// <summary>
    /// The id of the site this used belongs to
    /// </summary>
    [MaxLength(255)]
    public string PrimarySite { get; set; }

    /// <summary>
    /// The id of the region this used belongs to
    /// </summary>
    [MaxLength(255)]
    public string PrimaryRegion { get; set; }
    
    /// <summary>
    /// Returns if the user is a mentor.
    /// </summary>
    [NotMapped]
    public bool IsMentor => Role != UserRoles.Student && Role != UserRoles.User && Role != UserRoles.CourseMaker;

    /// <summary>
    /// Returns if the user is a supervisor.
    /// </summary>
    [NotMapped]
    public bool IsSupervisor => Role is UserRoles.Supervisor or UserRoles.Admin;

    /// <summary>
    /// Returns if the user is a student.
    /// </summary>
    [NotMapped]
    public bool IsStudent => Role is UserRoles.User or UserRoles.Student;

    /// <summary>
    /// Returns if the user is a supervisor.
    /// </summary>
    [NotMapped]
    public bool IsCourseMakerOrAdmin => Role is UserRoles.Admin or UserRoles.CourseMaker;

    /// <summary>
    /// All the course this user (if he is a mentor) is involved in
    /// </summary>
    [InverseProperty("Mentor")]
    public ICollection<MentorCourse> MentorCourses { get; set; }

    /// <summary>
    /// All the enrollment of the student for which this user is an accountability partner
    /// </summary>
    [InverseProperty("AccountabilityPartner")]
    public ICollection<Enrollment> PartnerEnrollments { get; set; }

    /// <summary>
    /// All the enrollment the student is/has been enrolled in
    /// </summary>
    [InverseProperty("Student")]
    public ICollection<Enrollment> Enrollments { get; set; }

    /// <summary>
    /// All the enrollments this user is mentoring
    /// </summary>
    [InverseProperty("Mentor")]
    public ICollection<Enrollment> Mentoring { get; set; }

    /// <summary>
    /// All the ratings for the mentor
    /// </summary>
    [InverseProperty("Mentor")]
    public ICollection<MentorRating> Ratings { get; set; }

    /// <summary>
    /// The settings for the mentor
    /// </summary>
    [InverseProperty("Mentor")]
    public MentorSettings MentorSettings { get; set; }

    /// <summary>
    /// A collection of all the received messages for the users
    /// </summary>
    [InverseProperty(nameof(ChannelMessage.Receiver))]
    public ICollection<ChannelMessage> ReceivedChannelMessages { get; set; }

    /// <summary>
    /// A collection of all the send-messages for the users
    /// </summary>
    [InverseProperty(nameof(Message.Sender))]
    public ICollection<Message> SendMessages { get; set; }

    /// <summary>
    /// A collection of all blogs the user wrote (admin only)
    /// </summary>
    [InverseProperty(nameof(Blog.Author))]
    public ICollection<Blog> Blogs { get; set; }

    /// <summary>
    /// A collection of rewards that are granted to the user
    /// </summary>
    [InverseProperty(nameof(RewardGrant.User))]
    public ICollection<RewardGrant> Grants { get; set; }

    /// <summary>
    /// This method returns the preferred interface language (or the nl default)
    /// </summary>
    public string GetPreferredInterfaceLanguage()
    {
        if (string.IsNullOrEmpty(PreferredInterfaceLanguage)) return "nl";
        return PreferredInterfaceLanguage.ToLower();
    }

    /// <summary>
    /// Backup keys for the user for when two-factor auth is enabled
    /// </summary>
    [InverseProperty("User")]
    public ICollection<UserBackupKey> BackupKeys { get; set; }
    
    /// <summary>
    /// A collection of the regions for which this user - if it is a mentor -
    /// is mentoring in.
    /// </summary>
    [InverseProperty(nameof(MentorRegion.Mentor))]
    public ICollection<MentorRegion> MentorRegions { get; set; }

    /// <summary>
    /// Create a new empty user
    /// </summary>
    public User()
    {
        MentorCourses = [];
        Enrollments = [];
        PartnerEnrollments = [];
        Mentoring = [];
        ReceivedChannelMessages = [];
    }
}