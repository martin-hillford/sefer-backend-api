namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class GetPublishedContentPageByPermalinkRequest(string permalink, string site) : IRequest<ContentPage>
{
    public readonly string Permalink = permalink?.ToLower().Trim();

    public readonly string Site = site?.ToLower().Trim();
}