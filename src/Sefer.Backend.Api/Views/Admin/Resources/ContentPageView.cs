// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.Resources;

/// <summary>
/// A view on the content page
/// </summary>
/// <inheritdoc />
public class ContentPageView : AbstractView<ContentPage>
{
    #region Properties

    /// <summary>
    /// The type of this page
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentPageType Type => Model.Type;

    /// <summary>
    /// Holds the sequence id (for menu page)
    /// </summary>
    public int? SequenceId => Model.SequenceId;

    /// <summary>
    /// Holds if the page is published
    /// </summary>
    public bool IsPublished => Model.IsPublished;

    /// <summary>
    /// The name or title of the page
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// The permalink for this page (optional, but recommended)
    /// </summary>
    public string Permalink => Model.Permalink;

    /// <summary>
    /// The content for the page
    /// </summary>
    public string Content => Model.Content;

    public bool IsHtmlContent => Model.IsHtmlContent;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public ContentPageView(ContentPage model)
        : base(model) { }

    #endregion
}