namespace Sefer.Backend.Api.Data.Requests.Blogs;

public class GetBlogByIdRequest(int? id) : GetEntityByIdRequest<Blog>(id);