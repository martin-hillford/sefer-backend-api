// This is a post model. And although it is never instantiated in code, it is in runtime 
// ReSharper disable UnusedAutoPropertyAccessor.Global ClassNeverInstantiated.Global
namespace Sefer.Backend.Api.Models.Admin.Resources;

/// <summary>
/// This class contains the model posted by an admin to update or add a content page
/// </summary>
public class ContentPagePostModel
{
    /// <summary>
    /// The type of this page
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentPageType Type { get; set; }

    /// <summary>
    /// Holds the sequence id (for menu page)
    /// </summary>
    public int? SequenceId { get; set; }

    /// <summary>
    /// Holds if the page is published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// The name or title of the page
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The permalink for this page (optional, but recommended)
    /// </summary>
    public string Permalink { get; set; }

    /// <summary>
    /// The content for the page
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Holds if the content is html or markdown
    /// </summary>
    public bool IsHtmlContent { get; set; }
}

/// <summary>
/// The ContentPagePostModelFactory is capable of converting from api model to database models
/// </summary>
public static class ContentPagePostModelFactory
{
    /// <summary>
    /// This method creates a new content page from the model
    /// </summary>
    /// <param name="model">The model to convert</param>
    /// <returns>A newly created content page</returns>
    public static ContentPage Create(ContentPagePostModel model)
    {
        return new ContentPage
        {
            Content = model.Content,
            CreationDate = DateTime.UtcNow,
            IsPublished = model.IsPublished,
            Name = model.Name,
            Permalink = model.Permalink,
            SequenceId = model.SequenceId,
            Type = model.Type,
            IsHtmlContent = model.IsHtmlContent
        };
    }

    /// <summary>
    /// This method patches the page with model
    /// </summary>
    /// <param name="page">The page to patch</param>
    /// <param name="model">The model with properties to patch the page with</param>
    public static void Patch(ContentPage page, ContentPagePostModel model)
    {
        page.Content = model.Content;
        page.IsPublished = model.IsPublished;
        page.Name = model.Name;
        page.Permalink = model.Permalink;
        page.SequenceId = model.SequenceId;
        page.Type = model.Type;
        page.ModificationDate = DateTime.UtcNow;
        page.IsHtmlContent = model.IsHtmlContent;
    }
}



