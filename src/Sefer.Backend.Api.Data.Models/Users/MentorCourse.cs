namespace Sefer.Backend.Api.Data.Models.Users;

/// <summary>
/// This model just models that a mentor can be a mentor of a course
/// </summary>
public class MentorCourse : Entity
{
    /// <summary>
    /// The id of the course the mentor is mentoring
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The id of the mentor is that mentoring the course
    /// </summary>
    public int MentorId { get; set; }

    /// <summary>
    /// The course the mentor is mentoring
    /// </summary>
    [ForeignKey("CourseId")]
    public Course Course { get; set; }

    /// <summary>
    /// The mentor is that mentoring the course
    /// </summary>
    [ForeignKey("MentorId")]
    public User Mentor { get; set; }
}
