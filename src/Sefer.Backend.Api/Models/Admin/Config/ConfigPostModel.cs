// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Config;

/// <summary>
/// Defines all the config variables to be posted
/// </summary>
public class ConfigPostModel
{
    /// <summary>
    /// A value between 0 and zero indicating the importance of age in the determine which mentor is most suitable
    /// </summary>
    [Range(0, 1)]
    public double RelativeAgeFactor { get; set; }

    /// <summary>
    /// The number of days that may between now and the last enrollment of the student that give the right to maintain the same mentor (if possible given the course)
    /// </summary>
    [Range(0, short.MaxValue)]
    public short SameMentorDays { get; set; }

    /// <summary>
    /// The optimal age difference between student and mentor in year
    /// </summary>
    public byte OptimalAgeDifference { get; set; }

    /// <summary>
    /// The number of days that a student will be counted as active after his last lessons submission
    /// </summary>
    [Range(0, short.MaxValue)]
    public short StudentActiveDays { get; set; }

    /// <summary>
    /// The number of days that are between the last logon day and the moment an automatic reminder
    /// </summary>
    [Range(0, short.MaxValue)]
    public short StudentReminderDays { get; set; }

    /// <summary>
    /// There should be always one mentor in the system. This is the id of the mentor that is used for backup.
    /// </summary>
    public int BackupMentorId { get; set; }

    /// <summary>
    /// Get/set the number of lessons per day a student is allowed to submit
    /// </summary>
    public byte? MaxLessonSubmissionsPerDay { get; set; }
    
    /// <summary>
    /// Get/set if during (personal) mentor assignment the mentor must have the same gender as the student.
    /// This is required is some contexts 
    /// </summary>
    public bool StrictGenderAssignment { get; set; }
    
    /// <summary>
    /// Gets/sets if a personal mentor must be assigned at the registration of the user.
    /// </summary>
    public bool AssignPersonalMentorOnRegistration  { get; set; }

    /// <summary>
    /// Converts the model to a data model
    /// </summary>
    /// <returns></returns>
    public Settings ToModel()
    {
        return new Settings
        {
            RelativeAgeFactor = RelativeAgeFactor,
            BackupMentorId = BackupMentorId,
            MaxLessonSubmissionsPerDay = MaxLessonSubmissionsPerDay,
            OptimalAgeDifference = OptimalAgeDifference,
            SameMentorDays = SameMentorDays,
            StudentActiveDays = StudentActiveDays,
            StudentReminderDays = StudentReminderDays,
            StrictGenderAssignment = StrictGenderAssignment,
            AssignPersonalMentorOnRegistration = AssignPersonalMentorOnRegistration
        };
    }
}
