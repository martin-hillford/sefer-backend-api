namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetPublishedSpecialPageByTypeHandler(IServiceProvider serviceProvider) : Handler<GetPublishedSpecialPageByTypeRequest, ContentPage>(serviceProvider)
{
    public override async Task<ContentPage> Handle(GetPublishedSpecialPageByTypeRequest request, CancellationToken token)
    {
        if (request.Type is ContentPageType.MenuPage or ContentPageType.IndividualPage) return null;

        // First check if a site specific page can found
        var specificPage = await GetSpecificPage(request, token);
        if (specificPage != null) return specificPage;

        // Find the normal case
        await using var context = GetDataContext();
        return await context.ContentPages
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Type == request.Type && p.IsPublished, token);
    }

    private async Task<ContentPage> GetSpecificPage(GetPublishedSpecialPageByTypeRequest request, CancellationToken token)
    {
        var site = request.Site?.ToLower().Trim();
        await using var context = GetDataContext();

        try
        {
            var specificPage = await context
                .Set<ContentPageOverride>()
                .FirstOrDefaultAsync(p => p.Type == request.Type && p.IsPublished && p.Site.ToLower() == site, token);
            return specificPage?.AsContentPage();
        }
        catch (Exception) { return null; }
    }
}