namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class GetPublishedSpecialPageByTypeRequest(ContentPageType type, string site) : IRequest<ContentPage>
{
    public readonly ContentPageType Type = type;

    public readonly string Site = site;
}