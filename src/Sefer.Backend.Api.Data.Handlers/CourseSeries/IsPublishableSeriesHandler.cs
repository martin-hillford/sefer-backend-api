namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class IsPublishableSeriesHandler(IServiceProvider serviceProvider)
    : Handler<IsPublishableSeriesRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsPublishableSeriesRequest request, CancellationToken token)
    {
        var series = await Send(new GetPublishedCoursesOfSeriesRequest(request.SeriesId), token);
        if (series == null) return false;
        return series.Count != 0;
    }
}