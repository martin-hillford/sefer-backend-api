namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class GetRegionsHandler(IServiceProvider serviceProvider) : Handler<GetRegionRequest, List<IRegion>>(serviceProvider)
{
    public override async Task<List<IRegion>> Handle(GetRegionRequest request, CancellationToken token)
    {
        var cached = Cache.Get<List<IRegion>>("database-regions");
        if (cached != null) return cached;

        var networkProvider = ServiceProvider.GetNetworkProvider();
        var regions = await networkProvider.GetRegionsAsync();
        Cache.Set("database-regions", regions);
        return regions;
    }
}