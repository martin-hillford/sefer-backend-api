namespace Sefer.Backend.Api.Data.Requests.ContentPages;

public class GetContentPageByPermalinkRequest(string permalink) : IRequest<ContentPage>
{
    public readonly string Permalink = permalink;
}