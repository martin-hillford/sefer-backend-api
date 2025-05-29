namespace Sefer.Backend.Api.Data.Requests.Blogs;

public class IsBlogNameUniqueRequest(int? blogId, string name) : IRequest<bool>
{
    public readonly int? BlogId = blogId;

    public readonly string Name = name;
}