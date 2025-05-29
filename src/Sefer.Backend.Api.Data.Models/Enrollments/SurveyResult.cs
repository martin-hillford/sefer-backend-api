// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength
namespace Sefer.Backend.Api.Data.Models.Enrollments;

/// <summary>
/// This models represents the result of the survey conduct after an enrollment to the student
/// </summary>
public class SurveyResult : Entity
{
    #region Properties

    /// <summary>
    /// The Date the result was created
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The rating given to the course (between 0 and 5)
    /// </summary>
    public int? CourseRatingId { get; set; }

    /// <summary>
    /// The rating given to the mentor (between 0 and 5)
    /// </summary>
    public int? MentorRatingId { get; set; }

    /// <summary>
    /// The enrollment of which survey this about
    /// </summary>
    public int EnrollmentId { get; set; }

    /// <summary>
    /// The testimony of the student about the course
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Holds if the student gives permission to use his testimony on social media
    /// </summary>
    public bool SocialPermissions { get; set; }

    /// <summary>
    /// Holds if the admin has processed the survey result
    /// </summary>
    public bool AdminProcessed { get; set; } = false;

    #endregion

    #region References

    /// <summary>
    /// The rating of the course that was provided by the student
    /// </summary>
    [ForeignKey(nameof(CourseRatingId))]
    public CourseRating CourseRating { get; set; }

    /// <summary>
    /// The rating of the mentor that was provided by the student
    /// </summary>
    [ForeignKey(nameof(MentorRatingId))]
    public MentorRating MentorRating { get; set; }

    /// <summary>
    /// The enrollment the survey was about
    /// </summary>
    [ForeignKey(nameof(EnrollmentId))]
    public Enrollment Enrollment {get; set;}

    #endregion
}