// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Shared.Courses;
using Sefer.Backend.Api.Views.Shared.Enrollments;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// Holds a view for the mentor on submission he has to review
/// </summary>
public class ReviewDetailView
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
    /// The id of the student that is taking the course
    /// </summary>
    public readonly int StudentId;

    /// <summary>
    /// The student that is taking the course
    /// </summary>
    public readonly MentorStudentInfoView Student;

    /// <summary>
    /// The date the submission was done
    /// </summary>
    public readonly DateTime SubmissionDate;

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
    public readonly ReadOnlyCollection<CorrectedAnswerView> Answers;

    /// <summary>
    /// Information about the course
    /// </summary>
    public readonly CourseDisplayView Course;

    /// <summary>
    /// The name of the mentor that is overseeing this submissions
    /// </summary>
    public string MentorName { get; private set; }

    /// <summary>
    /// The id of the mentor that is overseeing this submissions
    /// </summary>
    public int? MentorId { get; private set; }

    /// <summary>
    /// A view on the lesson that the submission belongs to
    /// </summary>
    public readonly LessonView Lesson;

    /// <summary>
    /// Create a new submission review view
    /// </summary>
    /// <remarks>The lesson, course, enrollment and student should be loaded for this view</remarks>
    public ReviewDetailView(LessonSubmission submission, List<CorrectedAnswerView> answers, MentorStudentData data, IFileStorageService fileStorageService, IAvatarService avatarService)
    {
        Id = submission.Id;
        EnrollmentId = submission.EnrollmentId;
        Lesson = new LessonView(submission.Lesson);
        Course = new CourseDisplayView(submission.Enrollment.CourseRevision.Course, fileStorageService);
        Student = new MentorStudentInfoView(submission.Enrollment.Student, data, avatarService);
        if(submission.SubmissionDate.HasValue) SubmissionDate = submission.SubmissionDate.Value;
        if (answers != null) Answers = answers.OrderBy(a => a.SequenceId).ToList().AsReadOnly();
        ReviewedByMentor = submission.ReviewDate != null;
        ReviewDate = submission.ReviewDate;
        MentorName = submission.Enrollment.Mentor?.Name;
        MentorId = submission.Enrollment.MentorId;
        StudentId = submission.Enrollment.StudentId;
    }
}