namespace Sefer.Backend.Api.Data.Requests.CourseSeries;

public class PublishSeriesRequest(Series series) : IRequest<bool>
{
    public readonly Series Series = series;
}