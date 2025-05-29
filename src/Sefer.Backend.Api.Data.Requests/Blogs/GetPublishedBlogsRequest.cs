namespace Sefer.Backend.Api.Data.Requests.Blogs;

public class GetPublishedBlogsRequest(int? take = null) : IRequest<List<Blog>>
{
    public readonly int? Take = take;
}