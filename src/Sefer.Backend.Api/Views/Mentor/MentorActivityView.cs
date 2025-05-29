// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// The ActivityView is a view used to display activity of the student.
/// </summary>
public class MentorActivityView
{
    /// <summary>
    /// The type of the activity
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly MentorActivityType Type;

    /// <summary>
    /// The time of the activity
    /// </summary>
    public readonly DateTime? Time;

    /// <summary>
    /// Create an activity from an loginLogEntry
    /// </summary>
    /// <param name="loginLogEntry"></param>
    public MentorActivityView(LoginLogEntry loginLogEntry)
    {
        Type = MentorActivityType.Login;
        Time = loginLogEntry.LogTime;
    }
}
