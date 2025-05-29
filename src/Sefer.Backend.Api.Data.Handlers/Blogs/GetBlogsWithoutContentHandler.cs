namespace Sefer.Backend.Api.Data.Handlers.Blogs;

public class GetBlogsWithoutContentHandler(IServiceProvider serviceProvider)
    : Handler<GetBlogsWithoutContentRequest, List<BlogBase>>(serviceProvider)
{
    public override async Task<List<BlogBase>> Handle(GetBlogsWithoutContentRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Blogs
            .AsNoTracking()
            .Select(b => new BlogBase
            {
                Id = b.Id,
                Permalink = b.Permalink,
                Name = b.Name,
                IsPublished = b.IsPublished,
                Author = b.Author,
                AuthorId = b.AuthorId,
                CreationDate = b.CreationDate,
                PublicationDate = b.PublicationDate,
                IsHtmlContent = b.IsHtmlContent
            })
            .ToListAsync(token);
    }
}