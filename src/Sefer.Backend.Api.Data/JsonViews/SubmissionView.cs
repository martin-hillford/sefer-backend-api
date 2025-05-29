namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// The student lesson submitted view is a view that is used to display the content of a message
/// that indicates that a student has sent a lesson for review to the mentor
/// </summary>
public class SubmissionView
{
    #region Properties

    /// <summary>
    /// Gets/sets the id of the submission that is saved
    /// </summary>
    public int SubmissionId { get; init; }

    /// <summary>
    /// The mentor can set that the results are visible for the student
    /// </summary>
    /// <returns></returns>
    public bool ResultsStudentVisible { get; init; }

    /// <summary>
    /// Holds if the enrollment is imported from the old system
    /// </summary>
    public bool Imported { get; init; }

    /// <summary>
    /// The course this lesson submission is about
    /// </summary>
    public CourseView Course { get; init; }

    /// <summary>
    /// The lesson this lesson submission is about
    /// </summary>
    public LessonView Lesson { get; init; }

    /// <summary>
    /// A list of all the answers provided by the student
    /// </summary>
    public List<AnswerView> Answers { get; init; } = [];

    /// <summary>
    /// The mentor of this submission (can theoretically be empty)
    /// </summary>
    public PrimitiveUserView Mentor { get; init; }

    /// <summary>
    /// The student that posted the submission
    /// </summary>
    public PrimitiveUserView Student { get; init; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new LessonSubmittedView
    /// </summary>
    public SubmissionView(LessonSubmission submission)
    {
        Course = new CourseView(submission.Lesson.CourseRevision.Course);
        Lesson = new LessonView(submission.Lesson);
        if (submission.Enrollment.Mentor != null) Mentor = new PrimitiveUserView(submission.Enrollment.Mentor);
        Student = new PrimitiveUserView(submission.Enrollment.Student);

        Imported = submission.Imported;
        ResultsStudentVisible = submission.ResultsStudentVisible;
        SubmissionId = submission.Id;

        if (submission.Imported) return;
        Answers = submission.Answers
            .Select(a => new AnswerView(a, submission.Lesson.GetQuestion(a)))
            .ToList();
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    public SubmissionView() { }

    #endregion
}