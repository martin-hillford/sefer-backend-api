// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Resources;

/// <summary>
/// This class contains the model posted by an admin to update or add a site specific content page
/// </summary>
public class SiteSpecificPageModel
{
    /// <summary>
    /// The id of the content page that is overridden by this content
    /// </summary>
    public int ContentPageId { get; set; }

    /// <summary>
    /// The content for the page
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Holds if the page is published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// The site this override is for
    /// </summary>
    public string Site { get; set; }
}