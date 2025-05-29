// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Admin.User;

/// <summary>
/// Holds a view on the student of a mentor
/// </summary>
public class MentorStudentView : UserListView
{
    /// <summary>
    /// Holds the active enrollment of the student
    /// </summary>
    protected readonly Data.Models.Enrollments.Enrollment Enrollment;

    /// <summary>
    /// Creates a view on the (active) enrollment and student
    /// </summary>
    /// <param name="enrollment">The enrollment that links the student to the mentor</param>
    public MentorStudentView(Data.Models.Enrollments.Enrollment enrollment) : base(enrollment.Student)
    {
        Enrollment = enrollment;
    }

    /// <summary>
    /// The current course the student is following
    /// </summary>
    public string Course => Enrollment?.CourseRevision?.Course?.Name;

    /// <summary>
    /// The date the user started to attend the course
    /// </summary>
    public DateTime? EnrollmentDate => Enrollment?.CreationDate;

    /// <summary>
    /// The number of completed lessons by the student
    /// </summary>
    public int? CompletedLessons => Enrollment?.LessonSubmissions?.Count(e => e.IsFinal);

    /// <summary>
    /// The number of reviewed lessons by the mentor
    /// </summary>
    public int? ReviewedLessons => Enrollment?.LessonSubmissions?.Count(e => e.ReviewDate.HasValue);
}