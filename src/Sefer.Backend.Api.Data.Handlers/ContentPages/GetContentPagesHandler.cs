namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetContentPagesHandler(IServiceProvider serviceProvider)
    : Handler<GetContentPagesRequest, List<ContentPage>>(serviceProvider)
{
    public override async Task<List<ContentPage>> Handle(GetContentPagesRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ContentPages
            .AsNoTracking()
            .OrderBy(p => p.Type).ThenBy(p => p.SequenceId)
            .ToListAsync(token);
    }
}