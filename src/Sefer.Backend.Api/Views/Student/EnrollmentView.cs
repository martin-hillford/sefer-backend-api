// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Views.Shared.Enrollments;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// An enrollment holds information on the enrollment of a user in a course
/// </summary>
/// <inheritdoc />
public class EnrollmentView : ExtendedEnrollmentView
{
    #region Properties

    /// <summary>
    /// Returns the next lesson in this enrollment
    /// </summary>
    public readonly LessonView NextLesson;

    #endregion

    #region Constructor

    public EnrollmentView(Enrollment enrollment, IFileStorageService fileStorageService) : base(enrollment, fileStorageService)
    {
        // deal with the lesson
        var sorted = enrollment.CourseRevision.Lessons.OrderBy(l => l.SequenceId).ToList();
        if (LessonCompletedCount < sorted.Count) NextLesson = new LessonView(sorted[LessonCompletedCount], enrollment, fileStorageService);
    }

    #endregion
}