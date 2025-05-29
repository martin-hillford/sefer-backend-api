// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Resources;

/// <summary>
/// A view on a blog
/// </summary>
public sealed class BlogView : AbstractBlogView
{
    /// <summary>
    /// The content for the blog
    /// </summary>
    public string Content => Model.Content;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public BlogView(Blog model) : base(model) { }
}