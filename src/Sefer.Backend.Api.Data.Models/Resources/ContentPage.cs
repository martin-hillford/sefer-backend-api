namespace Sefer.Backend.Api.Data.Models.Resources;

/// <summary>
/// Represents a page, which are admin editable information pages about the system
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity"/>
/// <inheritdoc cref="IPermaLinkable"/>
public class ContentPage : ModifyDateLogEntity, IPermaLinkable
{
    /// <summary>
    /// The type of this page
    /// </summary>
    public ContentPageType Type { get; set; }

    /// <summary>
    /// Holds the sequence id (for menu page)
    /// </summary>
    /// <value></value>
    public int? SequenceId { get; set; }

    /// <summary>
    /// Holds if the page is published
    /// </summary>
    /// <value></value>
    public bool IsPublished { get; set; }

    /// <summary>
    /// The name or title of the page
    /// </summary>
    /// <inheritdoc />
    [MinLength(3), MaxLength(255), Required]
    public string Name { get; set; }

    /// <summary>
    /// The permalink for this page (optional, but recommended)
    /// </summary>
    /// <inheritdoc />
    [MaxLength(255), PermalinkFormat]
    public string Permalink { get; set; }

    /// <summary>
    /// The content for the page
    /// </summary>
    [MinLength(1), Required, MaxLength(int.MaxValue)]
    public string Content { get; set; }

    /// <summary>
    /// Returns if the content is html or markdown (plain text)
    /// </summary>
    public bool IsHtmlContent { get; set; } = true;
}