namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class GetRegionByIdHandler(IServiceProvider serviceProvider) : Handler<GetRegionByIdRequest, IRegion>(serviceProvider)
{
    public override async Task<IRegion> Handle(GetRegionByIdRequest request, CancellationToken token)
    {
        if (string.IsNullOrEmpty(request.Id)) return null;
        var sites = await Send(new GetRegionRequest(), token);
        return sites.FirstOrDefault(v => v.Id == request.Id);
    }
}