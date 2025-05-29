namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// The settings for the mentor
/// </summary>
/// <inheritdoc />
public class MentorSettings : Entity
{
    /// <summary>
    /// The id of the mentor these settings do apply for
    /// </summary>
    public int MentorId { get; set; }

    /// <summary>
    /// The id of the supervisor for this mentor
    /// </summary>
    public int? SupervisorId { get; set; }

    /// <summary>
    /// The maximum number of students the mentor can handle
    /// </summary>
    [Range(0, short.MaxValue)]
    public short MaximumStudents { get; set; }

    /// <summary>
    /// The preferred number of students the mentor can handle
    /// </summary>
    [Range(0, short.MaxValue)]
    public short PreferredStudents { get; set; }

    /// <summary>
    /// At least one mentor in the system should an overflow mentor, to deal with situation for which everybody in occupied
    /// </summary>
    public bool AllowOverflow { get; set; }

    /// <summary>
    /// The mentor of the setting
    /// </summary>
    [ForeignKey("MentorId")]
    public User Mentor { get; set; }

    /// <summary>
    /// The supervisor of the mentor
    /// </summary>
    [ForeignKey("SupervisorId")]
    public User Supervisor { get; set; }

    /// <summary>
    /// This bool holds if the mentor is a personal mentor,
    /// that is, he cannot be assigned to public students.
    /// </summary>
    public bool IsPersonalMentor { get; set; }
}
