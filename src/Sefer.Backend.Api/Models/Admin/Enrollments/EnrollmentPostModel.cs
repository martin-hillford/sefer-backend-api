// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Enrollments;

/// <summary>
/// This class is used to post new enrollments by the admin
/// </summary>
public class EnrollmentPostModel
{
    /// <summary>
    /// The id of the student that took the course
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// The id of the course the student took
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The (optional) id of the mentor that gave the course.
    /// When set to null - self study is assumed
    /// </summary>
    public int? MentorId { get; set; }

    /// <summary>
    /// When set to true, the enrollment is completed and a grade and Completion Date have to be set
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// When set to true, the enrollment will be marked as 'made on paper'
    /// </summary>
    public bool OnPaper { get; set; }

    /// <summary>
    /// The grade of the student
    /// </summary>
    /// <remarks>Between 1 and 10</remarks>
    public byte? Grade { get; set; }

    /// <summary>
    /// The date the course was completed
    /// </summary>
    public DateTime? CompletionDate { get; set; }
}