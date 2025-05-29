namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetPublishedCoursesBySeriesRequest(int seriesId) : IRequest<List<Course>>
{
    public readonly int SeriesId = seriesId;
}