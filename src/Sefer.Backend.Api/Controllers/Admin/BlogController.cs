using Sefer.Backend.Api.Models.Admin.Course;
using Sefer.Backend.Api.Models.Admin.Resources;
using Sefer.Backend.Api.Views.Admin.Resources;
using Sefer.Backend.Api.Views.Shared;

namespace Sefer.Backend.Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class BlogController(IServiceProvider provider) : BaseController(provider)
{
    [HttpGet("/admin/content/blogs/base")]
    public async Task<ActionResult<List<Blog>>> GetBlogs()
    {
        var blogs = await Send(new GetBlogsWithoutContentRequest());
        return Ok(blogs);
    }

    [HttpGet("/admin/content/blogs/{id:int}")]
    public async Task<ActionResult<BlogView>> GetBlog(int id)
    {
        var blog = await Send(new GetBlogWithAuthorRequest(id));
        if (blog == null) return NotFound();
        var view = new BlogView(blog);
        return Ok(view);
    }

    [HttpDelete("/admin/content/blogs/{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var blog = await Send(new GetBlogByIdRequest(id));
        if (blog == null) return NotFound();

        var deleted = await Send(new DeleteBlogRequest(blog));
        return deleted ? StatusCode(204) : Problem("There was a problem with deleting the blog.");
    }

    [HttpPut("/admin/content/blogs/{id:int}/publish")]
    public async Task<ActionResult> Publish(int id)
    {
        var blog = await Send(new GetBlogByIdRequest(id));
        if (blog == null) return NotFound();

        blog.IsPublished = true;
        blog.PublicationDate ??= DateTime.UtcNow;

        var updated = await Send(new UpdateBlogRequest(blog));
        return updated ? Ok(blog) : Problem("There was a problem with publishing the blog.");
    }

    [HttpPut("/admin/content/blogs/{id:int}/take-offline")]
    public async Task<ActionResult> UnPublish(int id)
    {
        var blog = await Send(new GetBlogByIdRequest(id));
        if (blog == null) return NotFound();
        blog.IsPublished = false;

        var updated = await Send(new UpdateBlogRequest(blog));
        return updated ? Ok(blog) : Problem("There was a problem with retracting the blog.");
    }

    [HttpPost("/admin/content/blogs")]
    public async Task<ActionResult<BlogView>> Insert([FromBody] BlogPostModel model)
    {
        // Validate the model
        var user = await GetCurrentUser();
        if (model == null || user == null) return BadRequest();
        var blog = BlogPostModelFactory.Create(model, user.Id);
        if (!await Send(new IsBlogValidRequest(blog))) return BadRequest();

        // Insert it into the database
        var added = await Send(new AddBlogRequest(blog));
        if (added == false) return Problem("Adding the blog failed.");

        // Return the result to the user
        var view = new BlogView(blog);
        return Created($"/admin/content/blogs/{view.Id}", view);
    }

    [HttpPut("/admin/content/blogs/{id:int}")]
    public async Task<ActionResult> Update([FromBody] BlogPostModel model, int id)
    {
        // Validate the model
        if (model == null) return BadRequest();
        var blog = await Send(new GetBlogByIdRequest(id));
        if (blog == null) return NotFound();

        BlogPostModelFactory.Patch(blog, model);
        if (!await Send(new IsBlogValidRequest(blog))) return BadRequest();

        // Update the database
        var updated = await Send(new UpdateBlogRequest(blog));
        return updated ? Ok() : Problem("Updating the blog failed.");
    }

    [HttpPost("/admin/content/blogs/permalink")]
    public async Task<ActionResult<BooleanView>> IsPermalinkUnique([FromBody] IsPermalinkUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var isUnique = await Send(new IsBlogPermalinkUniqueRequest(post.Id, post.Permalink));
        var view = new BooleanView(isUnique);
        return Ok(view);
    }

    [HttpPost("/admin/content/blogs/name")]
    public async Task<IActionResult> IsNameUnique([FromBody] IsNameUniquePostModel post)
    {
        if (post == null) return Json(new BooleanView { Response = true });
        var isUnique = await Send(new IsBlogNameUniqueRequest(post.Id, post.Name));
        var view = new BooleanView(isUnique);
        return Ok(view);
    }
}