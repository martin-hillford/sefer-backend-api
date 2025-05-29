namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetPublishedMenuPagesLinksHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedMenuPagesLinksRequest, List<ContentPageLink>>(serviceProvider)
{
    public override async Task<List<ContentPageLink>> Handle(GetPublishedMenuPagesLinksRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ContentPages
            .AsNoTracking()
            .Where(p => p.IsPublished && p.Type == ContentPageType.MenuPage)
            .OrderBy(p => p.SequenceId)
            .Select(p => new ContentPageLink { Id = p.Id, Permalink = p.Permalink, Name = p.Name, IsPublished = p.IsPublished, Type = p.Type })
            .ToListAsync(token);
    }
}