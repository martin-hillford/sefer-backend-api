// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Mentor;

/// <summary>
/// The view of the mentor on the student
/// </summary>
public class MentorSettingsPostModel
{
    /// <summary>
    /// The maximum number of students the mentor can handle
    /// </summary>
    [Range(1, short.MaxValue)]
    public short MaximumStudents { get; set; }

    /// <summary>
    /// The preferred number of students the mentor can handle
    /// </summary>
    [Range(1, short.MaxValue)]
    public short PreferredStudents { get; set; }

    /// <summary>
    /// At least one mentor in the system should an overflow mentor, to deal with situation for which everybody in occupied
    /// </summary>
    public bool AllowOverflow { get; set; }
    
    /// <summary>
    /// Gets/sets e-mail preference of the user (no, notification, daily digest, weekly digest)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NotificationPreference NotificationPreference { get; set; }
}