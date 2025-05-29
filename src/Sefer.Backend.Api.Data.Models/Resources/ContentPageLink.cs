// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Resources;

/// <summary>
/// A simple structure to deal with links to pages
/// </summary>
public struct ContentPageLink
{
    /// <summary>
    /// The unique identifier for the page
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name or title of the page
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The permalink for this page (optional, but recommended)
    /// </summary>
    public string Permalink { get; set; }

    /// <summary>
    /// Holds if the page is published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// The type of this page
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContentPageType Type { get; set; }
}
