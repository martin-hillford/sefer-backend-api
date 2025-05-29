namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// A view on the current lesson
/// </summary>
public class CurrentLessonView : LessonView
{
    /// <summary>
    /// The (optional) submission
    /// </summary>
    public readonly LessonSubmissionView Submission;

    /// <summary>
    /// The current page the student is visiting
    /// </summary>
    public readonly int CurrentPage;
    
    /// <summary>
    /// The current audio track the student is listening to.
    /// </summary>
    public readonly int AudioTrack;

    /// <summary>
    /// Holds if the user is doing the study as self study
    /// </summary>
    public readonly bool IsSelfStudy;

    /// <summary>
    /// Creates a new View
    /// </summary>
    public CurrentLessonView(Lesson lesson, Enrollment enrollment, LessonSubmission submission, IFileStorageService fileStorageService) : base(lesson, enrollment, fileStorageService)
    {
        Submission = submission == null ? null : new LessonSubmissionView(submission);

        CurrentPage = submission?.CurrentPage ?? 1;
        IsSelfStudy = enrollment.IsSelfStudy;
        AudioTrack = submission?.AudioTrack ?? 0;
    }
}