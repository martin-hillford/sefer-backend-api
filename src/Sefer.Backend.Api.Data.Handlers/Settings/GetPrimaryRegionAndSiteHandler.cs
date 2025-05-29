namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class GetPrimaryRegionAndSiteHandler(IServiceProvider serviceProvider)
    : Handler<GetPrimaryRegionAndSiteRequest, (IRegion, ISite)>(serviceProvider)
{
    public override async Task<(IRegion, ISite)> Handle(GetPrimaryRegionAndSiteRequest request, CancellationToken token)
    {
        var user = await Send(new GetUserByIdRequest(request.UserId), token);
        if (user == null) return default;

        var region = await Send(new GetRegionByIdRequest(user.PrimaryRegion), token);
        var site = await Send(new GetSiteByNameRequest(user.PrimarySite), token);
        return (region, site);
    }
}