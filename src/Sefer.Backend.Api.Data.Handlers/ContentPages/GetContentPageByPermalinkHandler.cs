namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class GetContentPageByPermalinkHandler(IServiceProvider serviceProvider)
    : Handler<GetContentPageByPermalinkRequest, ContentPage>(serviceProvider)
{
    public override async Task<ContentPage> Handle(GetContentPageByPermalinkRequest request, CancellationToken token)
    {
        var permalink = request.Permalink?.ToLower().Trim();
        if (string.IsNullOrEmpty(permalink)) return null;

        await using var context = GetDataContext();
        return await context.ContentPages
            .AsNoTracking()
            .Where(p => p.Permalink.ToLower() == permalink)
            .OrderBy(p => p.SequenceId)
            .FirstOrDefaultAsync(token);
    }
}