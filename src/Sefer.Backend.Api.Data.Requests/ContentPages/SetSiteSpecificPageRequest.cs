namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class SetSiteSpecificPageRequest : IRequest<SiteSpecificContentPage>
{
    /// <summary>
    /// The id of the content page that is overridden by this content
    /// </summary>
    public int ContentPageId { get; init; }

    /// <summary>
    /// The content for the page
    /// </summary>
    public string Content { get; init; }

    /// <summary>
    /// Holds if the page is published
    /// </summary>
    public bool IsPublished { get; init; }

    /// <summary>
    /// The site this override is for
    /// </summary>
    public string Site { get; init; }
}