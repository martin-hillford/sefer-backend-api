namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class GetPublishedBlogsHandler(IServiceProvider serviceProvider)
    : Handler<GetPublishedBlogsRequest, List<Blog>>(serviceProvider)
{
    public override async Task<List<Blog>> Handle(GetPublishedBlogsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var query = context.Blogs
            .AsNoTracking()
            .Where(b => b.IsPublished == true && b.PublicationDate != null)
            .Include(b => b.Author)
            .OrderByDescending(b => b.PublicationDate);
        if (request.Take.HasValue) return await query.Take(request.Take.Value).ToListAsync(token);
        return await query.ToListAsync(token);
    }
}