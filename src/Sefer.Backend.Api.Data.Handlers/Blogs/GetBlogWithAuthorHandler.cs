namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class GetBlogWithAuthorHandler(IServiceProvider serviceProvider)
    : Handler<GetBlogWithAuthorRequest, Blog>(serviceProvider)
{
    public override async Task<Blog> Handle(GetBlogWithAuthorRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Blogs
            .AsNoTracking()
            .Include(b => b.Author)
            .SingleOrDefaultAsync(b => b.Id == request.BlogId, token);
    }
}