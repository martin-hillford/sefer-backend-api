// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Enrollments;

/// <summary>
/// The lessons submission is a data about a student (via the enrollment) who submitted a lesson
/// to a mentor or for self study. It also is used for saving results meanwhile
/// </summary>
public class LessonSubmission : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// The id of the enrollment the submission is part of
    /// </summary>
    public int EnrollmentId { get; set; }

    /// <summary>
    /// The id of the lesson this submission is about
    /// </summary>
    public int LessonId { get; set; }

    /// <summary>
    /// When final and when the course is self-study a grade is assigned (between 0 and 1)
    /// </summary>
    public double? Grade { get; set; }

    /// <summary>
    /// When set to true, the lesson is completed and can't be edited anymore
    /// </summary>
    public bool IsFinal { get; set; }

    /// <summary>
    /// The mentor can set that the results are visible for the student
    /// </summary>
    /// <returns></returns>
    public bool ResultsStudentVisible { get; set; }

    /// <summary>
    /// When the submission is final this property contains the submission date
    /// </summary>
    public DateTime? SubmissionDate { get; set; }

    /// <summary>
    /// The date that the mentor has reviewed the submission
    /// </summary>
    public DateTime? ReviewDate { get; set; }

    /// <summary>
    /// Holds if the submission is imported from the old system
    /// </summary>
    /// <value></value>
    public bool Imported { get; set; }

    /// <summary>
    /// A store for the current page
    /// </summary>
    public int CurrentPage { get; set; } = 1;
    
    /// <summary>
    /// A store for the current audio track
    /// </summary>
    public int AudioTrack { get; set; }
    
    #endregion

    #region References

    /// <summary>
    /// A reference to the lesson of this submission
    /// </summary>
    [ForeignKey(nameof(LessonId))]
    public Lesson Lesson { get; set; }

    /// <summary>
    /// A reference to the enrollment of this submission
    /// </summary>
    [ForeignKey("EnrollmentId")]
    public Enrollment Enrollment { get; set; }

    /// <summary>
    /// A list of all the answers added to this submission
    /// </summary>
    public ICollection<QuestionAnswer> Answers { get; set; }

    #endregion
}