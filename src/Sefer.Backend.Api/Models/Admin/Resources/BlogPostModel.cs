// This is a post model. And although it is never instantiated in code, it is in runtime 
// ReSharper disable UnusedAutoPropertyAccessor.Global ClassNeverInstantiated.Global
namespace Sefer.Backend.Api.Models.Admin.Resources;

/// <summary>
/// This class contains the model posted by an admin to update or add a blog
/// </summary>
public class BlogPostModel
{
    /// <summary>
    /// The name or title of the blog
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The permalink for this blog (optional, but recommended)
    /// </summary>
    public string Permalink { get; set; }

    /// <summary>
    /// The content for the blog
    /// </summary>
    public string Content { get; set; }

    public bool IsHtmlContent { get; set; }
}

/// <summary>
/// The BlogModelFactory is capable of converting from api model to database models
/// </summary>
public static class BlogPostModelFactory
{
    /// <summary>
    /// This method creates a new content blog from the model
    /// </summary>
    /// <param name="model">The model to convert</param>
    /// <param name="userId">The userId that created the blog</param>
    /// <returns>A newly created content blog</returns>
    public static Blog Create(BlogPostModel model, int userId)
    {
        return new Blog
        {
            Content = model.Content,
            CreationDate = DateTime.UtcNow,
            IsPublished = false,
            Name = model.Name,
            Permalink = model.Permalink,
            AuthorId = userId,
            IsHtmlContent = model.IsHtmlContent
        };
    }

    /// <summary>
    /// This method patches the blog with model
    /// </summary>
    /// <param name="blog">The blog to patch</param>
    /// <param name="model">The model with properties to patch the blog with</param>
    public static void Patch(Blog blog, BlogPostModel model)
    {
        blog.Content = model.Content;
        blog.Name = model.Name;
        blog.Permalink = model.Permalink;
        blog.ModificationDate = DateTime.UtcNow;
        blog.IsHtmlContent = model.IsHtmlContent;
    }
}