namespace Sefer.Backend.Api.Views.Student.EnrollmentOverview;

/// <summary>
/// An interface for the view on a submission
/// </summary>
public interface ISubmissionResultView
{
    #region Properties

    /// <summary>
    /// Gets is corrected answers are included
    /// </summary>
    bool CorrectAnswersIncluded { get; }

    /// <summary>
    /// A view on the lesson that the submission belongs to
    /// </summary>
    LessonView Lesson { get; }

    /// <summary>
    /// The date the (final) submission was done
    /// </summary>
    DateTime SubmissionDate { get; }

    /// <summary>
    /// The name of the course that this submission is about
    /// </summary>
    string CourseName { get; }

    /// <summary>
    /// The name of the student that posted this submissions
    /// </summary>
    string StudentName { get; }

    /// <summary>
    /// The name of the mentor that is overseeing this submissions
    /// </summary>
    string MentorName { get; }

    #endregion
}