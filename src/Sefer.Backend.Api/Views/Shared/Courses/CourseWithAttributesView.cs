// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
namespace Sefer.Backend.Api.Views.Shared.Courses;

/// <summary>
/// An extended view on the lessons
/// </summary>
/// <inheritdoc />
public abstract class CourseWithAttributesView : CourseDisplayView
{
    /// <summary>
    /// A reference to the public course revision
    /// </summary>
    protected readonly CourseRevision CourseRevision;

    /// <summary>
    /// The version of the revision (increasing number)
    /// </summary>
    public int Version => CourseRevision.Version;

    /// <summary>
    /// When this is set to true; Student are allowed to take this course
    /// without the aid of a mentor
    /// </summary>
    public bool AllowSelfStudy => CourseRevision.AllowSelfStudy;

    /// <summary>
    /// Gets if the rating of the course
    /// </summary>
    public readonly byte Rating;

    /// <summary>
    /// Gets the number of rating of the course
    /// </summary>
    public readonly int RatingCount;

    /// <summary>
    /// The number of students that ever followed this course
    /// </summary>
    public readonly long StudentCount;

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
    protected CourseWithAttributesView(CourseRevision courseRevision, CourseReadingTime readingTime, (byte Rating, int Count) ratings, IFileStorageService fileStorageService, int studentCount) : base(courseRevision.Course, fileStorageService)
    {
        CourseRevision = courseRevision;
        Rating = ratings.Rating;
        RatingCount = ratings.Count;
        StudentCount = studentCount;
        ReadingTime = readingTime.ReadingTime;
        LessonCount = readingTime.LessonCount;
        AverageReadingTime = readingTime.AverageReadingTime;
    }
}
