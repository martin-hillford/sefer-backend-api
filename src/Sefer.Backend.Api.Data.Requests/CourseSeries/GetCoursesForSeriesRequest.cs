namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class GetCoursesForSeriesRequest(int seriesId) : IRequest<List<Course>>
{
    public readonly int SeriesId = seriesId;
}