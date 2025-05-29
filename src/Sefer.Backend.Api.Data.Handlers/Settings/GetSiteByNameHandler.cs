namespace Sefer.Backend.Api.Data.Handlers.Settings;

public class GetSiteByNameHandler(IServiceProvider serviceProvider) : Handler<GetSiteByNameRequest, ISite>(serviceProvider)
{
    public override async Task<ISite> Handle(GetSiteByNameRequest request, CancellationToken token)
    {
        if (string.IsNullOrEmpty(request.Name)) return null;
        var sites = await Send(new GetSitesRequest(), token);
        return sites.FirstOrDefault(v => v.Hostname == request.Name);
    }
}