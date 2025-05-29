namespace Sefer.Backend.Api.Data.Requests.Blogs;

public class GetPublishedBlogByPermalinkRequest(string permalink) : IRequest<Blog>
{
    public readonly string Permalink = permalink;
}