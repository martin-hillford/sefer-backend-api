namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// Holds reading time information of a course.
/// Only based on published revisions
/// </summary>
public class CourseReadingTime
{
    /// <summary>
    /// The identifier of the course the reading time is about
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The identifier of the published course revision of the course
    /// </summary>
    public int RevisionId { get; set; }

    /// <summary>
    /// The reading time of the full course
    /// </summary>
    [Precision(18, 6)]
    public decimal ReadingTime { get; set; }

    /// <summary>
    /// The number of lessons in the course
    /// </summary>
    public int LessonCount { get; set; }

    /// <summary>
    /// The average reading time per lesson
    /// </summary>
    [Precision(18, 6)]
    public decimal AverageReadingTime { get; set; }
}