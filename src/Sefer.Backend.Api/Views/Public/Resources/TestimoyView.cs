using Sefer.Backend.Api.Data.JsonViews;
// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Resources;

/// <summary>
/// A basic view on testimonies
/// </summary>
public sealed class TestimonyView : AbstractView<Testimony>
{
    /// <summary>
    /// The testimony itself
    /// </summary>
    public string Content => Model.Content;

    /// <summary>
    /// The (optional) name of the person who made this testimony
    /// </summary>
    public string Name => (IsAnonymous == false) ? Model.Name : string.Empty;

    /// <summary>
    /// True when the testimony was anonymous
    /// </summary>
    public bool IsAnonymous => Model.IsAnonymous;

    /// <summary>
    /// Create a new view from a given testimony
    /// </summary>
    /// <param name="model"></param>
    public TestimonyView(Testimony model) : base(model) { }
}