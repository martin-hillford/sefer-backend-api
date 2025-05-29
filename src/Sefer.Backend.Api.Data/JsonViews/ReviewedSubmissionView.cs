// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// The student lesson submitted view is a view that is used to display the content of a message
/// that indicates that a student has sent a lesson for review to the mentor
/// </summary>
public class ReviewedSubmissionView
{
    #region Properties

    /// <summary>
    /// Holds the user of the view
    /// </summary>
    protected readonly User User;

    /// <summary>
    /// Gets/sets the id of the submission that is saved
    /// </summary>
    /// <returns></returns>
    public int SubmissionId { get; set; }

    /// <summary>
    /// The course this lesson submission is about
    /// </summary>
    public CourseView Course { get; set; }

    /// <summary>
    /// The lesson this lesson submission is about
    /// </summary>
    public LessonView Lesson { get; set; }

    /// <summary>
    /// When final and when the course is self-study a grade is assigned
    /// </summary>
    public double? Grade { get; set; }

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
    public bool Imported { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new LessonSubmittedView
    /// </summary>
    public ReviewedSubmissionView(LessonSubmission submission, User user = null)
    {
        User = user;
        SubmissionId = submission.Id;
        Grade = submission.Grade;
        ResultsStudentVisible = submission.ResultsStudentVisible;
        SubmissionDate = submission.SubmissionDate;
        ReviewDate = submission.ReviewDate;
        Imported = submission.Imported;
        Course = new CourseView(submission.Lesson.CourseRevision.Course);
        Lesson = new LessonView(submission.Lesson);
    }

    #endregion
}