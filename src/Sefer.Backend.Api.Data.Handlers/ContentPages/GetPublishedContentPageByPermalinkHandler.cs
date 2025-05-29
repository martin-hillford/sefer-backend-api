namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetPublishedContentPageByPermalinkHandler(IServiceProvider serviceProvider) : Handler<GetPublishedContentPageByPermalinkRequest, ContentPage>(serviceProvider)
{
    public override async Task<ContentPage> Handle(GetPublishedContentPageByPermalinkRequest request, CancellationToken token)
    {
        if (string.IsNullOrEmpty(request.Permalink)) return null;

        // First check if a specific content page can be found
        var specificPage = await GetSiteSpecificPage(request, token);
        if (specificPage != null) return specificPage;

        // Next check if a regular content page can be found
        await using var context = GetDataContext();
        return await context.ContentPages
            .AsNoTracking()
            .Where(p => p.IsPublished && p.Permalink.ToLower() == request.Permalink)
            .OrderBy(p => p.SequenceId)
            .FirstOrDefaultAsync(token);
    }

    private async Task<ContentPage> GetSiteSpecificPage(GetPublishedContentPageByPermalinkRequest request, CancellationToken token)
    {
        try
        {
            await using var context = GetDataContext();
            var specificPage = await context.Set<ContentPageOverride>()
                .Where(p =>
                    p.IsPublished &&
                    p.Permalink.ToLower() == request.Permalink &&
                    p.Site.ToLower() == request.Site
                )
                .OrderBy(p => p.SequenceId)
                .FirstOrDefaultAsync(token);
            return specificPage?.AsContentPage();
        }
        catch (Exception) { return null; }
    }
}