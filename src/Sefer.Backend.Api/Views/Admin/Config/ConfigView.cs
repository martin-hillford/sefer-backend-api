using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.Config;

/// <summary>
/// A view on the configuration
/// </summary>
/// <inheritdoc />
public class ConfigView : AbstractView<Settings>
{
    /// <summary>
    /// A value between 0 and zero indicating the importance of age in the determine which mentor is most suitable
    /// </summary>
    public double RelativeAgeFactor => Model.RelativeAgeFactor;

    /// <summary>
    /// The number of days that may between now and the last enrollment of the student that give the right to maintain the same mentor (if possible given the course)
    /// </summary>
    public short SameMentorDays => Model.SameMentorDays;

    /// <summary>
    /// The optimal age difference between student and mentor in year
    /// </summary>
    public byte OptimalAgeDifference => Model.OptimalAgeDifference;

    /// <summary>
    /// The number of days that a student will be counted as active after his last lessons submission
    /// </summary>
    public short StudentActiveDays => Model.StudentActiveDays;

    /// <summary>
    /// The number of days that are between the last logon day and the moment an automatic reminder (once!0 will be sent to the user
    /// </summary>
    public short StudentReminderDays => Model.StudentReminderDays;

    /// <summary>
    /// There should be always one mentor in the system. This is the id of the mentor that is used for backup.
    /// </summary>
    public int BackupMentorId => Model.BackupMentorId;

    /// <summary>
    /// Get/set the number of lessons per day a student is allowed to submit
    /// </summary>
    public byte? MaxLessonSubmissionsPerDay => Model.MaxLessonSubmissionsPerDay;

    /// <summary>
    /// A value between 0 and zero indicating the importance of availability in the determine which mentor is most suitable
    /// </summary>
    public double RelativeAvailabilityFactor => Model.RelativeAvailabilityFactor;

    /// <summary>
    /// Holds is the lesson submissions are limited
    /// </summary>
    public bool IsLessonSubmissionsLimited => Model.IsLessonSubmissionsLimited;
    
    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public ConfigView(Settings model) : base(model)  { }
}
