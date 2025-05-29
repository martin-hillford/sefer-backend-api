namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class DeleteSeriesRequest(Series series) : IRequest<bool>
{
    public readonly Series Series = series;
}