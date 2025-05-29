// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Student.Profile;

/// <summary>
/// This class contains the settings
/// </summary>
public class StudentSettingsPostModel
{
    /// <summary>
    /// Gets/sets e-mail preference of the user (no, notification, daily digest, weekly digest)
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public NotificationPreference NotificationPreference { get; set; }
    
    /// <summary>
    /// Indicates the user prefers spoken courses
    /// </summary>
    public bool PreferSpokenCourses { get; set; }
}