namespace Sefer.Backend.Api.Data.Requests.Resources;

public class InsertShortUrlRequest(ShortUrl shortUrl) : IRequest<bool>
{
    public readonly ShortUrl ShortUrl = shortUrl;
}