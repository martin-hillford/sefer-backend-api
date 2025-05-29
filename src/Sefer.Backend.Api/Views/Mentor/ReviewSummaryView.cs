// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Shared.Enrollments;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// Holds a view for the mentor on submission he has to review
/// </summary>
public class ReviewSummaryView
{
    /// <summary>
    /// The id of the submission
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The id of the course enrollment this submission belongs to
    /// </summary>
    public readonly int EnrollmentId;

    /// <summary>
    /// The id the lesson that the student submitted;
    /// </summary>
    public readonly int LessonId;

    /// <summary>
    /// The name the lesson that the student submitted;
    /// </summary>
    public readonly string Lesson;

    /// <summary>
    /// The id of the course the student is following
    /// </summary>
    public readonly int CourseId;

    /// <summary>
    /// The course the student is taking
    /// </summary>
    public readonly string Course;

    /// <summary>
    /// The id of the student that is taking the course
    /// </summary>
    public readonly int StudentId;

    /// <summary>
    /// The name of the student that is taking the course
    /// </summary>
    public readonly string StudentName;

    /// <summary>
    /// The date the submission was done
    /// </summary>
    public readonly DateTime? SubmissionDate;

    /// <summary>
    /// The avatar url for the student
    /// </summary>
    public readonly string StudentAvatarUrl;

    /// <summary>
    /// Contains if this is an active student or not
    /// </summary>
    public readonly bool? StudentActive;

    /// <summary>
    /// Holds the review date for this submission
    /// </summary>
    public readonly DateTime? ReviewDate;

    /// <summary>
    /// Holds if this submission is review by a mentor
    /// </summary>
    public readonly bool ReviewedByMentor;

    /// <summary>
    /// A list of answers given by the student
    /// </summary>
    /// <value></value>
    public readonly ReadOnlyCollection<CorrectedAnswerView> Answers;

    /// <summary>
    /// Create a new submission review view
    /// </summary>
    /// <remarks>The lesson, course, enrollment and student should be loaded for this view</remarks>
    public ReviewSummaryView(LessonSubmission submission, bool? studentActive, IAvatarService avatarService) : this(submission, null, studentActive, avatarService) { }

    /// <summary>
    /// Create a new submission review view
    /// </summary>
    /// <remarks>The lesson, course, enrollment and student should be loaded for this view</remarks>
    public ReviewSummaryView(LessonSubmission submission, List<CorrectedAnswerView> answers, bool? studentActive, IAvatarService avatarService)
    {
        Id = submission.Id;
        EnrollmentId = submission.EnrollmentId;
        LessonId = submission.LessonId;
        Lesson = submission.Lesson.Number + ": " + submission.Lesson.Name;
        CourseId = submission.Enrollment.CourseRevision.CourseId;
        Course = submission.Enrollment.CourseRevision.Course.Name;
        StudentId = submission.Enrollment.StudentId;
        StudentName = submission.Enrollment.Student.Name;
        SubmissionDate = submission.SubmissionDate;
        if (answers != null) Answers = answers.OrderBy(a => a.SequenceId).ToList().AsReadOnly();
        ReviewedByMentor = submission.ReviewDate != null;
        ReviewDate = submission.ReviewDate;
        StudentAvatarUrl = avatarService.GetAvatarUrl(StudentId, StudentName);
        StudentActive = studentActive;
    }
}