// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
namespace Sefer.Backend.Api.Views.Public.Users;

/// <summary>
/// View to returns site information to client
/// </summary>
/// <remarks>
/// Creates a new view on the site
/// </remarks>
/// <param name="site">the site</param>
public sealed class LogonSiteView(ISite site)
{
    /// <summary>
    /// The underlying site object
    /// </summary>
    private readonly ISite _site = site;

    /// <summary>
    /// A unique identifier for the site
    /// </summary>
    public string Id => _site.Hostname;

    /// <summary>
    /// This is the site for this site (the homepage of that site)
    /// </summary>
    public string SiteUrl => _site.SiteUrl;

    /// <summary>
    /// The display name for site
    /// </summary>    
    public string Name => _site.Name;

    /// <summary>
    /// The short, user-friendly, url of the site
    /// </summary>
    public string ShortUrl => _site.Hostname;
}