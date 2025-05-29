namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetContentPageLinksHandler(IServiceProvider serviceProvider)
    : Handler<GetContentPageLinksRequest, List<ContentPageLink>>(serviceProvider)
{
    public override async Task<List<ContentPageLink>> Handle(GetContentPageLinksRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var pages = await context.ContentPages
            .AsNoTracking()
            .OrderBy(p => p.Type).ThenBy(p => p.SequenceId)
            .Select(p => new { p.Id, p.Name, p.Permalink, p.IsPublished, p.Type })
            .ToListAsync(token);

        var links = pages
            .Select(p =>
                new ContentPageLink
                {
                    Id = p.Id,
                    Permalink = p.Permalink,
                    Name = p.Name,
                    IsPublished = p.IsPublished,
                    Type = p.Type
                })
            .ToList();
        return links;
    }
}