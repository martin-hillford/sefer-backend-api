namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class GetPublishedBlogByPermalinkHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedBlogByPermalinkRequest, Blog>(serviceProvider)
{
    public override async Task<Blog> Handle(GetPublishedBlogByPermalinkRequest request, CancellationToken token)
    {
        var permalink = request?.Permalink?.ToLower().Trim();
        if (string.IsNullOrEmpty(permalink)) return null;

        var context = GetDataContext();
        return await context.Blogs.AsNoTracking()
            .Where(p => p.IsPublished && p.Permalink.ToLower() == permalink)
            .Include(b => b.Author)
            .FirstOrDefaultAsync(token);
    }
}