namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// The view of the mentor on the student
/// </summary>
public class MentorSettingsView
{
    /// <summary>
    /// The maximum number of students the mentor can handle
    /// </summary>
    public readonly short MaximumStudents;

    /// <summary>
    /// The preferred number of students the mentor can handle
    /// </summary>
    public readonly short PreferredStudents;

    /// <summary>
    /// At least one mentor in the system should an overflow mentor, to deal with situation for which everybody in occupied
    /// </summary>
    public readonly bool AllowOverflow;
    
    /// <summary>
    /// Gets/sets e-mail preference of the user (no, notification, daily digest, weekly digest)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly NotificationPreference NotificationPreference;
    
    /// <summary>
    /// Creates the view of the mentor on the student
    /// </summary>
    /// <param name="mentor"></param>
    /// <param name="settings"></param>
    public MentorSettingsView(User mentor, MentorSettings settings)
    {
        MaximumStudents = settings.MaximumStudents;
        PreferredStudents = settings.PreferredStudents;
        AllowOverflow = settings.AllowOverflow;
        NotificationPreference = mentor.NotificationPreference;
    }
}