// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Home;

/// <summary>
/// This is the data required for a page log
/// </summary>
public class PageLogPostModel
{
    /// <summary>
    /// The time (in ms since epoch) that page was visited
    /// </summary>
    public long Time { get; set; }

    /// <summary>
    /// The location of the page (location.pathname)
    /// </summary>
    public string Page { get; set; }

    /// <summary>
    /// The height of the screen
    /// </summary>
    public int ScreenHeight { get; set; }

    /// <summary>
    /// The width of the screen
    /// </summary>
    public int ScreenWidth { get; set; }
    
    /// <summary>
    /// Defines which site the user is using
    /// </summary>
    public string Site { get; set; }

    /// <summary>
    /// Defines which region the user is using
    /// </summary>
    public string Region { get; set; }

    /// <summary>
    /// (Optional) the user has entered using the given ad champagne
    /// </summary>
    public string Cmp { get; set; }
}
