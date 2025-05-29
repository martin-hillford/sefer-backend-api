// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Student.Profile;

/// <summary>
/// The Submission models the data that a user should provide for a lesson submission
/// </summary>
/// <remarks>Please now that the lesson id and the submission id is deducted from the student context</remarks>
public class SubmissionPostModel
{
    /// <summary>
    /// A list with the answers provided by the student on the question of the lesson
    /// </summary>
    /// <returns></returns>
    public List<QuestionAnswerPostModel> Answers { get; set; }

    /// <summary>
    /// Gets/sets if this submission is only a draft (no validation) or a final submission
    /// </summary>
    public bool Final { get; set; }
    
    /// <summary>
    /// Creates a new LessonSubmission
    /// </summary>
    /// <returns></returns>
    public LessonSubmission CreateNew(Enrollment enrollment, Lesson lesson)
    {
        return new LessonSubmission
        {
            EnrollmentId = enrollment.Id,
            IsFinal = Final,
            LessonId = lesson.Id,
        };
    }
}