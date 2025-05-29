using Sefer.Backend.Api.Views.Public.Resources;

namespace Sefer.Backend.Api.Controllers.Public;

public class BlogController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/public/blogs/recent")]
    [ResponseCache(Duration = 86400)]
    public Task<ActionResult<List<BlogBaseView>>> GetRecentBlogs() => GetRecentBlogs(10);

    [HttpGet("/public/blogs/recent/{take:int}")]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<List<BlogBaseView>>> GetRecentBlogs(int? take)
    {
        var blogs = await Send(new GetPublishedBlogsRequest(take ?? 10));
        return Json(blogs.Select(b => new BlogBaseView(b)));
    }

    [HttpGet("/public/blogs/{permalink}")]
    [ResponseCache(Duration = 86400)]
    public async Task<ActionResult<BlogView>> GetPublishedBlog(string permalink)
    {
        var blog = await Send(new GetPublishedBlogByPermalinkRequest(permalink));
        if (blog == null) return NotFound();
        var view = new BlogView(blog);
        return Json(view);
    }
}