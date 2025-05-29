namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class IsBlogNameUniqueHandler(IServiceProvider serviceProvider)
    : Handler<IsBlogNameUniqueRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsBlogNameUniqueRequest request, CancellationToken token)
    {
        var name = request?.Name?.ToLower().Trim();
        if (string.IsNullOrEmpty(name)) return true;

        var context = GetDataContext();
        return !await context.Blogs
            .AsNoTracking()
            .AnyAsync(sr => sr.Name.ToLower().Trim() == name && sr.Id != request.BlogId, token);
    }
}