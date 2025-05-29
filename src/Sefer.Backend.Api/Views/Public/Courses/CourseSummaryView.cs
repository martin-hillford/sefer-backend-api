using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Public.Courses;

/// <summary>
/// This view used for displaying summaries of courses (for series / homepage / all courses)
/// </summary>
public sealed class CourseSummaryView : CourseDisplayView
{
    /// <summary>
    /// The number of students that ever followed this course
    /// </summary>
    public long StudentCount { get; init; }

    /// <summary>
    /// The reading time of the full course
    /// </summary>
    public readonly decimal ReadingTime;

    /// <summary>
    /// The number of lessons in the course
    /// </summary>
    public readonly int LessonCount;

    /// <summary>
    /// The average reading time per lesson
    /// </summary>
    public readonly decimal AverageReadingTime;

    /// <summary>
    /// Creates a new View
    /// </summary>
    public CourseSummaryView(Course model, CourseReadingTime readingTime, IFileStorageService fileStorageService, long studentCount) : base(model, fileStorageService)
    {
        StudentCount = studentCount;
        ReadingTime = readingTime.ReadingTime;
        LessonCount = readingTime.LessonCount;
        AverageReadingTime = readingTime.AverageReadingTime;
    }
}