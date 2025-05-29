namespace Sefer.Backend.Api.Data.Models.Resources;

/// <summary>
/// The blog represents a blog that can be added to talk about news or backgrounds
/// </summary>
public class Blog : BlogBase
{
    /// <summary>
    /// The content for the page
    /// </summary>
    [MinLength(1), Required, MaxLength(int.MaxValue)]
    public string Content { get; set; }
}