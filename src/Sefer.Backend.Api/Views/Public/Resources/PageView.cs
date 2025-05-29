using Sefer.Backend.Api.Data.JsonViews;
// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Resources;

/// <summary>
/// View on pages
/// </summary>
public sealed class PageView : AbstractView<ContentPage>
{
    /// <summary>
    /// The content of the page
    /// </summary>
    public string Content => Model.Content;

    /// <summary>
    /// The name of the page
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Returns is content is html content (true) or markdown (false)
    /// </summary>
    public bool IsHtmlContent => Model.IsHtmlContent;

    /// <summary>
    /// Constructor
    /// </summary>
    public PageView(ContentPage model) : base(model) { }
}