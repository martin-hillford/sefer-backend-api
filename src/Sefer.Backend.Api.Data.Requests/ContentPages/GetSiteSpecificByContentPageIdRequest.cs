namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class GetSiteSpecificByContentPageIdRequest(int contentPageId, string site) : IRequest<SiteSpecificContentPage>
{
    public readonly int ContentPageId = contentPageId;

    public readonly string Site = site?.ToLower().Trim();
}