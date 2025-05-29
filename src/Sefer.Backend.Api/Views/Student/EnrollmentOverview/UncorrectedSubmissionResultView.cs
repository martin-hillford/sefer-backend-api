namespace Sefer.Backend.Api.Views.Student.EnrollmentOverview;

/// <summary>
/// A view on a submission that is not corrected by the mentor
/// </summary>
public class UncorrectedSubmissionResultView : BaseSubmissionResultView, ISubmissionResultView
{
    /// <summary>
    /// Gets is corrected answers are included
    /// </summary>
    public bool CorrectAnswersIncluded => false;

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
    public string CourseName { get; protected init; }

    /// <summary>
    /// The name of the student that posted this submissions
    /// </summary>
    public string StudentName { get; protected set; }

    /// <summary>
    /// The name of the mentor that is overseeing this submissions
    /// </summary>
    public string MentorName { get; protected set; }

    /// <summary>
    /// Creates a view on submission that is not (yet) correct by a mentor
    /// </summary>
    public UncorrectedSubmissionResultView(LessonSubmission submission, Enrollment enrollment, IFileStorageService fileStorageService) : base(enrollment.IsSelfStudy, enrollment.Id)
    {
        if (submission.SubmissionDate.HasValue) SubmissionDate = submission.SubmissionDate.Value;
        Lesson = new LessonView(submission.Lesson, enrollment, fileStorageService);
        SubmissionId = submission.Id;
        ResultsStudentVisible = submission.ResultsStudentVisible;

        if (enrollment.Student != null) StudentName = enrollment.Student.Name;
        MentorName = (enrollment.IsSelfStudy || enrollment.Mentor == null) ? string.Empty : enrollment.Mentor.Name;
        if (enrollment.CourseRevision is { Course: not null }) CourseName = enrollment.CourseRevision.Course.Name;
    }
}