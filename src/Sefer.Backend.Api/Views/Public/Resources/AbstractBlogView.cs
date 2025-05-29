using Sefer.Backend.Api.Data.JsonViews;
// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Resources;

/// <summary>
/// An abstract base for  blog views
/// </summary>
public abstract class AbstractBlogView : AbstractView<Blog>
{
    /// <summary>
    /// Holds the date this blog was published (for the first time)
    /// </summary>
    public DateTime? PublicationDate => Model.PublicationDate;

    /// <summary>
    /// The name or title of the blog
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// The permalink for this blog (optional, but recommended)
    /// </summary>
    public string Permalink => Model.Permalink;

    /// <summary>
    /// The name of the author that wrote the blog
    /// </summary>
    public readonly string AuthorName;

    /// <summary>
    /// Gets is an author is set for this blog
    /// </summary>
    public bool HasAuthor => String.IsNullOrEmpty(AuthorName) == false;

    /// <summary>
    /// Gets if the given content is html or markdown content
    /// </summary>
    public bool IsHtmlContent => Model.IsHtmlContent;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    protected AbstractBlogView(Blog model) : base(model)
    {
        if (model.Author != null) AuthorName = model.Author.Name;
    }
}