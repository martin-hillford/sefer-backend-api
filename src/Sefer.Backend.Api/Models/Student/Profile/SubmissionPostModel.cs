// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Diagnostics.CodeAnalysis;

namespace Sefer.Backend.Api.Models.Student.Profile;

/// <summary>
/// The Submission models the data that a user should provide for a lesson submission
/// </summary>
/// <remarks>
/// In the first implementations, it was only possible to have one active course at the same time.
/// In other projects it was found that it may be better to have multiple courses active.
/// To deal with this, the enrollmentId for this submission can be null. If not set, the
/// backend will check that only one enrollment is active. If set, it can deal with multiple
/// </remarks>
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class SubmissionPostModel
{
    /// <summary>
    /// A list with the answers provided by the student on the question of the lesson
    /// </summary>
    /// <returns></returns>
    public List<QuestionAnswerPostModel> Answers { get; set; }
    
    /// <summary>
    /// The enrollment to which this submission belongs.
    /// If set to null, only one enrollment can be active.
    /// </summary>
    public int? EnrollmentId { get; set; }

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