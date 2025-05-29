namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class GetSitesHandler(IServiceProvider serviceProvider) : Handler<GetSitesRequest, IEnumerable<ISite>>(serviceProvider)
{
    public override async Task<IEnumerable<ISite>> Handle(GetSitesRequest request, CancellationToken token)
    {
        var cached = Cache.Get<IEnumerable<ISite>>("database-sites");
        if (cached != null) return cached;

        var networkProvider = ServiceProvider.GetNetworkProvider();
        var sites = await networkProvider.GetSitesAsync();
        Cache.Set("database-sites", sites);
        return sites;
    }
}