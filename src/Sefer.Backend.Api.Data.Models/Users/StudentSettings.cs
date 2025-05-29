namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// The settings for the student
/// </summary>
/// <inheritdoc />
public class StudentSettings : Entity
{
    /// <summary>
    /// The id of the student these settings do apply for
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// The id of the personal mentor of the
    /// </summary>
    public int? PersonalMentorId { get; set; }

    /// <summary>
    /// The mentor of the setting
    /// </summary>
    [ForeignKey("StudentId")]
    public User Student { get; set; }

    /// <summary>
    /// The mentor of the setting
    /// </summary>
    [ForeignKey("PersonalMentorId")]
    public User PersonalMentor { get; set; }
}
