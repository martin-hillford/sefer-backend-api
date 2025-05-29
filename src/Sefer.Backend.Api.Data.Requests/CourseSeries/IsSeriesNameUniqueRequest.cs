namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class IsSeriesNameUniqueRequest(int? seriesId, string name) : IRequest<bool>
{
    public readonly int? SeriesId = seriesId;

    public readonly string Name = name;
}