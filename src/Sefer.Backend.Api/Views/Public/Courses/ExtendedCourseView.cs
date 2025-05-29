using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Public.Courses;

/// <summary>
/// An extended view on the lessons
/// </summary>
/// <inheritdoc />
/// Todo: refactor because there are too many constructor arguments
public class ExtendedCourseView : CourseWithAttributesView
{
    /// <summary>
    /// Holds the number of lessons in the course
    /// </summary>
    public readonly List<LessonView> Lessons;

    /// <summary>
    /// Creates a new View
    /// </summary>
    public ExtendedCourseView(CourseRevision courseRevision, CourseReadingTime readingTime, IEnumerable<Lesson> lessons, (byte Rating, int Count) ratings, IFileStorageService fileStorageService, int studentCount)
        : base(courseRevision, readingTime, ratings, fileStorageService, studentCount)
    {
        Lessons = lessons?.Select(l => new LessonView(l)).ToList() ?? new();
    }
}
