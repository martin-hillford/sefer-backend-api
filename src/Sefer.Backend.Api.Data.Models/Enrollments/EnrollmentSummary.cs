// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Enrollments;

/// <summary>
/// This class represents a row of the EnrollmentSummaries view.
/// This view holds for each student it current enrollment and
/// information about the course and each next lesson.
/// </summary>
public class EnrollmentSummary
{
    /// <summary>
    /// The identifier of the enrollment
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The time the enrollment was created (thus the date the user enrolled for the course)
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The id of the student who enrolled for the course
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// The name of the student who enrolled for the course
    /// </summary>
    public string StudentName { get; set; }

    /// <summary>
    /// The id of the mentor that the student is assigned to.
    /// This can be null for self studies
    /// </summary>
    public int? MentorId { get; set; }

    /// <summary>
    /// Holds if the enrollment is a self study course
    /// </summary>
    public bool IsSelfStudy => MentorId == null;

    /// <summary>
    /// The name of the mentor that the student is assigned to.
    /// </summary>
    public string MentorName { get; set; }

    /// <summary>
    /// The id of the course revision the user is following
    /// </summary>
    public int CourseRevisionId { get; set; }

    /// <summary>
    /// The id of the course the user is following
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The name of the course the user is following
    /// </summary>
    public string CourseName { get; set; }

    /// <summary>
    /// The number of lesson that the user so far have submitted
    /// if this null, then the submitted lesson are zero
    /// </summary>
    public int? Submitted { get; set; }

    /// <summary>
    /// The total number of lessons in the course
    /// </summary>
    public int LessonCount { get; set; }

    /// <summary>
    /// The id of the next lesson that the student has to follow
    /// Can be null if it is the enrollment is completed
    /// </summary>
    public int? NextLessonId { get; set; }

    /// <summary>
    /// The name of the next lesson that the student has to follow
    /// Can be null if it is the enrollment is completed
    /// </summary>
    public string NextLessonName { get; set; }

    /// <summary>
    /// The date and time the user was last active.
    /// Can be used for sorting
    /// </summary>
    public DateTime StudentLastActive { get; set; }

    /// <summary>
    /// Contains if this enrollment is active (thus is not closed)
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Holds the date this enrollment was closed
    /// </summary>
    public DateTime? ClosureDate { get; set; }

    /// <summary>
    /// Holds if for this enrollment the course is completed
    /// </summary>
    public bool IsCourseCompleted { get; set; }

    /// <summary>
    /// Holds if the admin has explicit allowed a retake
    /// </summary>
    public bool AllowRetake { get; set; }

    /// <summary>
    /// For each student the enrollments are 'ranked'. The higher the rank,
    /// the more recent it was created. Rank = 1 is the last created enrollment by the user
    /// </summary>
    [JsonIgnore]
    public long Rank { get; set; }
}