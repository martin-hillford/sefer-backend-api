namespace Sefer.Backend.Api.Data.Requests.Blogs;

public class GetBlogWithAuthorRequest(int blogId) : IRequest<Blog>
{
    public readonly int BlogId = blogId;
}