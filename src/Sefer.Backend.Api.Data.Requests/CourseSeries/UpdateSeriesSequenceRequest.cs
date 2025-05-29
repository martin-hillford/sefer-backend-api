namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class UpdateSeriesSequenceRequest(List<int> series) : IRequest<bool>
{
    public readonly List<int> Series = series;
}