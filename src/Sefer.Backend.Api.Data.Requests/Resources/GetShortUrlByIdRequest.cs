namespace Sefer.Backend.Api.Data.Requests.Resources;

public class GetShortUrlByIdRequest(string id) : IRequest<ShortUrl>
{
    public readonly string Id = id;
}