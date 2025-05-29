namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class SetSiteSpecificPageHandler(IServiceProvider serviceProvider)
    : Handler<SetSiteSpecificPageRequest, SiteSpecificContentPage>(serviceProvider)
{
    public override async Task<SiteSpecificContentPage> Handle(SetSiteSpecificPageRequest request, CancellationToken token)
    {
        try
        {
            // First step is to check if the site and the content page for request exists
            var contentPage = await Send(new GetContentPageByIdRequest(request.ContentPageId), token);
            var site = await Send(new GetSiteByNameRequest(request.Site), token);
            if (site == null || contentPage == null) return null;

            // Now determine if we have an insert or an update
            await using var context = GetDataContext();
            var specificPage = await context.SiteSpecificContentPages
                .FirstOrDefaultAsync(x => x.ContentPageId == contentPage.Id && x.Site == request.Site, cancellationToken: token)
                ?? new SiteSpecificContentPage { CreationDate = DateTime.UtcNow };

            specificPage.Content = request.Content;
            specificPage.Site = request.Site;
            specificPage.ContentPageId = request.ContentPageId;
            specificPage.IsPublished = request.IsPublished;
            specificPage.ModificationDate = DateTime.UtcNow;

            if (specificPage.Id == 0) context.SiteSpecificContentPages.Add(specificPage);
            await context.SaveChangesAsync(token);

            return specificPage;
        }
        catch (Exception) { return null; }
    }
}