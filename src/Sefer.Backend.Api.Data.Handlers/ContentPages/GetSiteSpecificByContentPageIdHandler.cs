namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetSiteSpecificByContentPageIdHandler(IServiceProvider serviceProvider)
    : Handler<GetSiteSpecificByContentPageIdRequest, SiteSpecificContentPage>(serviceProvider)
{
    public override async Task<SiteSpecificContentPage> Handle(GetSiteSpecificByContentPageIdRequest request, CancellationToken token)
    {
        if (string.IsNullOrEmpty(request.Site)) return null;
        await using var context = GetDataContext();
        return await context.SiteSpecificContentPages
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ContentPageId == request.ContentPageId && p.Site == request.Site, token);
    }
}








