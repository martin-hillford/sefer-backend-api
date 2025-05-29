using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Public.Users;

/// <summary>
/// An enrollment holds information on the enrollment of a user in a course
/// </summary>
/// <inheritdoc />
public sealed class EnrollmentView : AbstractView<Enrollment>
{
    /// <summary>
    /// The student that is enrolled in the course
    /// </summary>
    private readonly User _student;

    /// <summary>
    /// TThe mentor that is leading the course
    /// </summary>
    private readonly User _mentor;

    /// <summary>
    /// The id of the student that is enrolled to the course.
    /// </summary>
    public int StudentId => _student.Id;

    /// <summary>
    /// The name of the student that is enrolled to the course.
    /// </summary>
    public string StudentName => _student.Name;

    /// <summary>
    /// The id of the mentor that is supporting the student enrolled to the course.
    /// </summary>
    public int? MentorId => _mentor?.Id;

    /// <summary>
    /// The name of the mentor that is supporting the student enrolled to the course.
    /// </summary>
    public string MentorName => _mentor?.Name;

    /// <summary>
    /// The id of the accountability partner that is supporting the student enrolled to the course.
    /// </summary>
    public int? AccountabilityPartnerId => Model.AccountabilityPartnerId;

    /// <summary>
    /// The date the enrollment is closed.
    /// Either because the user ended his enrollment or did complete the course
    /// </summary>
    public DateTime? ClosureDate => Model.ClosureDate;

    /// <summary>
    /// Holds if the User has completed the course. Can be set because the user submitted all the lessons of course to mentor.
    /// Or can be set by an administrator because he knows of completion in the past.
    /// </summary>
    public bool IsCourseCompleted => Model.IsCourseCompleted;

    /// <summary>
    /// Gets if the enrollment is active
    /// </summary>
    public bool IsActive => Model.IsActive;

    /// <summary>
    /// The course of this enrollment
    /// </summary>
    public readonly CourseDisplayView Course;

    /// <summary>
    /// Returns if this enrollment is a self study enrollment
    /// </summary>
    public bool IsSelfStudy => Model.IsSelfStudy;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="enrollment">The enrollment of the view</param>
    /// <param name="course">The course the enrollment is about</param>
    /// <param name="student">The student that is enrolled in the course</param>
    /// <param name="mentor">The mentor that is leading the course</param>
    /// <param name="storageService">The service for retrieve files</param>
    /// <inheritdoc />
    public EnrollmentView(Enrollment enrollment, Course course, User student, User mentor, IFileStorageService storageService) : base(enrollment)
    {
        Course = new CourseDisplayView(course, storageService);
        _student = student;
        _mentor = mentor;
    }
}
