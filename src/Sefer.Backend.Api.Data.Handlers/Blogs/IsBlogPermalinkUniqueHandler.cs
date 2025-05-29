namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class IsBlogPermalinkUniqueHandler(IServiceProvider serviceProvider)
    : Handler<IsBlogPermalinkUniqueRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsBlogPermalinkUniqueRequest request, CancellationToken token)
    {
        var permalink = request?.Permalink?.ToLower().Trim();
        if (string.IsNullOrEmpty(permalink)) return true;

        var context = GetDataContext();
        return !await context.Blogs
            .AsNoTracking()
            .AnyAsync(sr => sr.Permalink.ToLower().Trim() == permalink && sr.Id != request.BlogId, token);
    }
}