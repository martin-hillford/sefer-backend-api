namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class PublishSeriesHandler(IServiceProvider serviceProvider)
    : Handler<PublishSeriesRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(PublishSeriesRequest request, CancellationToken token)
    {
        if (request.Series == null) return false;
        if (!await Send(new IsPublishableSeriesRequest(request.Series.Id), token)) return false;
        request.Series.IsPublic = true;
        return await Send(new UpdateSeriesRequest(request.Series), token);
    }
}