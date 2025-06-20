// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Enrollments;

/// <summary>
/// An enrollment is a User following a Course support by a Mentor
/// </summary>
/// <inheritdoc />
public class Enrollment : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// The id of the revision of the course that is taken by the student.
    /// </summary>
    [Required]
    [InsertOnly]
    public int CourseRevisionId { get; set; }

    /// <summary>
    /// The id of the student that is enrolled to the course.
    /// </summary>
    [Required]
    [InsertOnly]
    public int StudentId { get; set; }

    /// <summary>
    /// The id of the mentor that is supporting the student enrolled to the course.
    /// </summary>
    public int? MentorId { get; set; }

    /// <summary>
    /// The id of the accountability partner that is supporting the student enrolled to the course.
    /// </summary>
    public int? AccountabilityPartnerId { get; set; }

    /// <summary>
    /// The date the enrollment is closed.
    /// Either because the user ended his enrollment or did complete the course
    /// </summary>
    public DateTime? ClosureDate { get; set; }

    /// <summary>
    /// Holds if the User has completed the course. Can be set because the user submitted all the lessons of course to mentor.
    /// Or can be set by an administrator because he knows of completion in the past.
    /// </summary>
    public bool IsCourseCompleted { get; set; }

    /// <summary>
    /// Holds is the user has submitted a survey for this enrollment
    /// </summary>
    /// <returns></returns>
    public bool SurveySubmitted { get; set; }

    /// <summary>
    /// The course that is followed by the student.
    /// </summary>
    [ForeignKey(nameof(CourseRevisionId))]
    [InverseProperty("Enrollments")]
    public CourseRevision CourseRevision { get; set; }

    /// <summary>
    /// The student that is enrolled in the course.
    /// </summary>
    [ForeignKey(nameof(StudentId))]
    [InverseProperty("Enrollments")]
    public User Student { get; set; }

    /// <summary>
    /// The mentor that is supporting the student enrolled to the course.
    /// </summary>
    [ForeignKey(nameof(MentorId))]
    [InverseProperty("Mentoring")]
    public User Mentor { get; set; }

    /// <summary>
    /// The accountability partner that is supporting the student enrolled to the course.
    /// </summary>
    [ForeignKey("AccountabilityPartnerId")]
    [InverseProperty("PartnerEnrollments")]
    public User AccountabilityPartner { get; set; }

    /// <summary>
    /// A list of all the submission of the lessons
    /// </summary>
    public ICollection<LessonSubmission> LessonSubmissions { get; set; } = [];

    /// <summary>
    /// Per feature request: student should be allowed to retake a course.
    /// </summary>
    public bool AllowRetake { get; set; }

    /// <summary>
    /// This contains the final grade for of the course (between 0 and zero)
    /// </summary>
    /// <value></value>
    public double? Grade { get; set; }

    /// <summary>
    /// Holds if the enrollment is imported from the old system
    /// </summary>
    /// <value></value>
    public bool Imported { get; set; }

    /// <summary>
    /// Holds if the enrollment was on paper
    /// </summary>
    /// <value></value>
    public bool OnPaper { get; set; }

    #endregion

    #region Derived Properties

    /// <summary>
    /// Gets if the enrollment is active
    /// </summary>
    [NotMapped]
    public bool IsActive => ClosureDate.HasValue == false || ClosureDate > DateTime.UtcNow;

    /// <summary>
    /// Returns if this is a self study enrollment
    /// </summary>
    [NotMapped]
    public bool IsSelfStudy => MentorId == null;

    /// <summary>
    /// A round value of the grade
    /// </summary>
    public double? GradeRounded => Grade.HasValue ?  Math.Round(Grade.Value * 100) / 100 : null;

    #endregion
}