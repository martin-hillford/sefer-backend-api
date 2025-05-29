using Sefer.Backend.Api.Views.Shared.Enrollments;

namespace Sefer.Backend.Api.Views.Student.EnrollmentOverview;

/// <summary>
/// A view on a submission that is corrected by the mentor
/// </summary>
public class CorrectedSubmissionResultView : BaseSubmissionResultView, ISubmissionResultView
{
    #region Properties

    /// <summary>
    /// The total number of answers given
    /// </summary>
    public readonly int TotalAnswersGiven;

    /// <summary>
    /// Gets the number of total correct answer
    /// </summary>
    /// <returns></returns>
    public readonly int CorrectAnswersGiven;

    /// <summary>
    /// Return the number of wrong answer given
    /// </summary>
    public int WrongAnswersGiven => TotalAnswersGiven - CorrectAnswersGiven;

    /// <summary>
    /// Holds (when the submission was final and the course is a self study course) the grade for this submission.null. It represents a value between 0 and 10
    /// </summary>
    /// <returns></returns>
    public readonly float? Grade;

    /// <summary>
    /// Gets is corrected answers are included
    /// </summary>
    public bool CorrectAnswersIncluded => true;

    /// <summary>
    /// A view on the lesson that the submission belongs to
    /// </summary>
    public LessonView Lesson { get; }

    /// <summary>
    /// The date the (final) submission was done
    /// </summary>
    public DateTime SubmissionDate { get; }

    /// <summary>
    /// The name of the course that this submission is about
    /// </summary>
    public string CourseName { get; private set; }

    /// <summary>
    /// The name of the student that posted this submissions
    /// </summary>
    public string StudentName { get; private set; }

    /// <summary>
    /// The name of the mentor that is overseeing this submissions
    /// </summary>
    public string MentorName { get; private set; }

    /// <summary>
    /// A list of correct answers
    /// </summary>
    public List<CorrectedAnswerView> CorrectedAnswers { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new view, this view will include a list of answers
    /// </summary>
    /// <inheritdoc />
    public CorrectedSubmissionResultView(LessonSubmission submission, List<CorrectedAnswerView> correctedAnswers, Enrollment enrollment, IFileStorageService fileStorageService) : base(enrollment)
    {
        CorrectedAnswers = correctedAnswers;
        TotalAnswersGiven = correctedAnswers.Count(a => a.QuestionType != ContentBlockTypes.QuestionOpen);
        CorrectAnswersGiven = correctedAnswers.Count(a => a.QuestionType != ContentBlockTypes.QuestionOpen && a.IsValid == true);

        if (submission.SubmissionDate.HasValue) SubmissionDate = submission.SubmissionDate.Value;
        Lesson = new LessonView(submission.Lesson, enrollment, fileStorageService);
        SubmissionId = submission.Id;
        if (submission.Grade.HasValue) Grade = (float)Math.Round(submission.Grade.Value * 10, 1);
        ResultsStudentVisible = submission.ResultsStudentVisible;

        if (enrollment.Student != null) StudentName = enrollment.Student.Name;
        MentorName = (enrollment.IsSelfStudy || enrollment.Mentor == null) ? string.Empty : enrollment.Mentor.Name;
        if (enrollment.CourseRevision is { Course: not null }) CourseName = enrollment.CourseRevision.Course.Name;
    }

    #endregion
}