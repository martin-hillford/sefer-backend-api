namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class GetRegionsHandler(IServiceProvider serviceProvider) : Handler<GetRegionRequest, IEnumerable<IRegion>>(serviceProvider)
{
    public override async Task<IEnumerable<IRegion>> Handle(GetRegionRequest request, CancellationToken token)
    {
        var cached = Cache.Get<IEnumerable<IRegion>>("database-regions");
        if (cached != null) return cached;

        var networkProvider = ServiceProvider.GetNetworkProvider();
        var regions = await networkProvider.GetRegionsAsync();
        Cache.Set("database-regions", regions);
        return regions;
    }
}