namespace Sefer.Backend.Api.Data.Handlers.ContentPages;

public class IsContentPageNameUniqueHandler(IServiceProvider serviceProvider)
    : Handler<IsContentPageNameUniqueRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsContentPageNameUniqueRequest request, CancellationToken token)
    {
        var name = request.Name?.ToLower().Trim();
        if (string.IsNullOrEmpty(name)) return true;

        await using var context = GetDataContext();
        return !await context.ContentPages
            .AsNoTracking()
            .Where(sr => sr.Name.ToLower().Trim() == name && sr.Id != request.ContentPageId)
            .AnyAsync(token);
    }
}