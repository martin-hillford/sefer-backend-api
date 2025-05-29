// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.Resources;

/// <summary>
/// A view on a blog
/// </summary>
public class BlogView : AbstractView<Blog>
{
    #region Properties

    /// <summary>
    /// Holds if the blog is published
    /// </summary>
    public bool IsPublished => Model.IsPublished;

    /// <summary>
    /// Holds the date this blog was published (for the first time)
    /// </summary>
    public DateTime? PublicationDate => Model.PublicationDate;

    /// <summary>
    /// Holds the date this blog was created
    /// </summary>
    public DateTime? CreationDate => Model.CreationDate;

    /// <summary>
    /// Holds the date this blog was modified for the last time
    /// </summary>
    public DateTime? ModificationDate => Model.ModificationDate;

    /// <summary>
    /// The name or title of the blog
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// The permalink for this blog (optional, but recommended)
    /// </summary>
    public string Permalink => Model.Permalink;

    /// <summary>
    /// The content for the blog
    /// </summary>
    public string Content => Model.Content;

    public bool IsHtmlContent => Model.IsHtmlContent;

    /// <summary>
    /// The name of the author that wrote the blog
    /// </summary>
    public readonly string AuthorName;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public BlogView(Blog model) : base(model)
    {
        if(model.Author != null) AuthorName = model.Author.Name;
    }

    #endregion
}