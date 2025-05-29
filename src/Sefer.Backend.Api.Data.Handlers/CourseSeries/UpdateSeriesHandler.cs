namespace Sefer.Backend.Api.Data.Handlers.CourseSeries;

public class UpdateSeriesHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateSeriesRequest, Series>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateSeriesRequest request, CancellationToken token)
    {
        if (request.Entity == null) return false;
        request.Entity.IsPublic &= await Send(new IsPublishableSeriesRequest(request.Entity.Id), token);
        return await base.Handle(request, token);
    }
}