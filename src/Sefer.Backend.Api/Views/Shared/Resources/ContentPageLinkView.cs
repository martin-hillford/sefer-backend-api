// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Views.Shared.Resources;

/// <summary>
/// A view on the content page that is a summary to create links to pages
/// </summary>
public class ContentPageLinkView
{
    #region Properties

    /// <summary>
    /// The id of the model
    /// </summary>
    public readonly int Id;

    /// <summary>
    /// The name or title of the page
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The permalink for this page (optional, but recommended)
    /// </summary>
    public readonly string Permalink;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="id">The id of the content page</param>
    /// <param name="name">The name of the content page</param>
    /// <param name="permalink">The permalink of the page</param>
    public ContentPageLinkView(int id, string name, string permalink)
    {
        Id = id;
        Name = name;
        Permalink = permalink;
    }

    #endregion
}