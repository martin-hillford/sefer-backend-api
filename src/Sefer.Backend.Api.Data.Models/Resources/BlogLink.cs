// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Resources;

/// <summary>
/// The base blog represents the base for a blog that can be added to talk about news or backgrounds
/// </summary>
public class BlogBase : ModifyDateLogEntity, IPermaLinkable
{
    #region Properties

    /// <summary>
    /// Holds if the blog is published
    /// </summary>
    /// <value></value>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Holds the date this blog was published (for the first time)
    /// </summary>
    public DateTime? PublicationDate { get; set; }

    /// <summary>
    /// The name or title of the blog
    /// </summary>
    /// <inheritdoc />
    [MinLength(2), MaxLength(255), Required]
    public string Name { get; set; }

    /// <summary>
    /// The name or title of the blog
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public string Title => Name;

    /// <summary>
    /// The permalink for this blog (optional, but recommended)
    /// </summary>
    /// <inheritdoc />
    [MaxLength(255), PermalinkFormat]
    public string Permalink { get; set; }

    /// <summary>
    /// The author that published the blog
    /// </summary>
    public int AuthorId { get; set; }

    public bool IsHtmlContent { get; set; }

    #endregion

    #region References

    /// <summary>
    /// The author that published the blog
    /// </summary>
    [ForeignKey(nameof(AuthorId))]
    public User Author { get; set; }

    #endregion
}