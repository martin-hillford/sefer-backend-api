namespace Sefer.Backend.Api.Views.Public.Courses;

/// <summary>
/// A container with courses and series
/// </summary>
public sealed class CourseListView
{
    /// <summary>
    /// A list of all the series (public available)
    /// </summary>
    public List<SeriesWithCourseSummaryView> Series { get; init; }

    /// <summary>
    /// A list of all the courses that are not in a series (public available)
    /// </summary>
    public List<PermalinkCourseView> Courses { get; init; }

    /// <summary>
    /// Create a new view
    /// </summary>
    public CourseListView()
    {
        Series = [];
        Courses = [];
    }
}
