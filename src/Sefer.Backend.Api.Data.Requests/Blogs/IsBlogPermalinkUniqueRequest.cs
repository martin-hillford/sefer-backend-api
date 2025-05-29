namespace Sefer.Backend.Api.Data.Requests.Blogs;

public class IsBlogPermalinkUniqueRequest(int? blogId, string permalink) : IRequest<bool>
{
    public readonly int? BlogId = blogId;

    public readonly string Permalink = permalink;
}