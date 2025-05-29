namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class IsPublishableSeriesRequest(int seriesId) : IRequest<bool>
{
    public readonly int SeriesId = seriesId;
}