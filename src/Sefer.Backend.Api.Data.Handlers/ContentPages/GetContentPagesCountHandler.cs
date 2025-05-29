namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetContentPagesCountHandler(IServiceProvider serviceProvider)
    : Handler<GetContentPagesCountRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetContentPagesCountRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ContentPages.CountAsync(token);
    }
}