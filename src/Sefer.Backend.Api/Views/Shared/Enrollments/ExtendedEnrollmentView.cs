// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global  MemberCanBeProtected.Global
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Shared.Enrollments;

/// <summary>
/// An enrollment holds information on the enrollment of a user in a course
/// </summary>
/// <inheritdoc />
public class ExtendedEnrollmentView : EnrollmentView
{
    #region Properties

    /// <summary>
    /// TThe mentor that is leading the course
    /// </summary>
    private readonly User _mentor;

    /// <summary>
    /// The id of the mentor that is supporting the student enrolled to the course.
    /// </summary>
    public int? MentorId => _mentor?.Id;

    /// <summary>
    /// The name of the mentor that is supporting the student enrolled to the course.
    /// </summary>
    public string MentorName => _mentor?.Name;

    /// <summary>
    /// The course of this enrollment
    /// </summary>
    public readonly CourseDisplayView Course;

    /// <summary>
    /// Returns the number of lessons in this enrollment
    /// </summary>
    public int LessonCount => Model.CourseRevision.Lessons.Count;

    /// <summary>
    /// Returns the number of lessons completed in this enrollment
    /// </summary>
    public int LessonCompletedCount
    {
        get
        {
            // Since the admin can add completed enrollment (from history or analog) check what to load
            if (HasLessons == false && Model.IsCourseCompleted) return LessonCount;
            return Model.LessonSubmissions.Count(l => l.IsFinal);
        }
    }

    /// <summary>
    /// This returns true when there are lessons loaded for a submission
    /// </summary>
    public bool HasLessons => Model.LessonSubmissions != null && Model.LessonSubmissions.Any();

    /// <summary>
    /// Returns the progress (as percentage) of the student in this enrollment
    /// </summary>
    public int Progress => (int)Math.Round(LessonCompletedCount / (double)LessonCount * 100.0);

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="enrollment">The enrollment of the view</param>
    /// <param name="fileStorageService" />
    /// <inheritdoc />
    public ExtendedEnrollmentView(Enrollment enrollment, IFileStorageService fileStorageService) : base(enrollment)
    {
        Course = new CourseDisplayView(enrollment.CourseRevision.Course, fileStorageService);
        _mentor = enrollment.Mentor;
    }

    #endregion
}
