// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Resources;

/// <summary>
/// A view on a blog with only a shorted summary
/// </summary>
public sealed class BlogBaseView : AbstractBlogView
{
    /// <summary>
    /// The content for the blog
    /// </summary>
    public readonly string Summary;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public BlogBaseView(Blog model) : base(model)
    {
        if (string.IsNullOrEmpty(model.Content)) return;

        const string pattern = @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>";
        Summary = Regex.Replace(Model.Content, pattern, string.Empty);

        if (model.Content.Length <= 320) return;

        Summary = Summary[..480];
        Summary = Summary[..Summary.LastIndexOf('.')] + ".";
    }
}