namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class SetSeriesForCourseRequest(int seriesId, List<Course> courses) : IRequest<bool>
{
    public readonly int SeriesId = seriesId;

    public readonly List<Course> Courses = courses;
}