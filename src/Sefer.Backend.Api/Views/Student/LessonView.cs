// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// The lesson model represent the current lesson for the user
/// </summary>
/// <inheritdoc cref="Public.Lessons.LessonView"/>
public class LessonView : Public.Lessons.LessonView
{
    #region Properties

    /// <summary>
    /// Holds the enrollment the lesson has it's context in
    /// </summary>
    protected readonly Enrollment Enrollment;

    /// <summary>
    /// The id of the enrollment that the student is currently following
    /// </summary>
    public int EnrollmentId => Enrollment.Id;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    public LessonView(Lesson lesson, Enrollment enrollment, IFileStorageService fileStorageService) : base(lesson, fileStorageService)
    {
        Enrollment = enrollment;
    }

    #endregion
}