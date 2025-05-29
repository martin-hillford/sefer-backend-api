using Sefer.Backend.Api.Data.Models.Resources;

namespace Sefer.Backend.Api.Services.ShortUrls;

public class ShortUrlService(IMediator mediator, IOptions<ShortUrlServiceOptions> options) : IShortUrlService
{
    private readonly ShortUrlServiceOptions _options = options.Value;

    public Task<(string shortUrl, string qrCode)> Create(ISite site, string path, DateTime? expires = null, string fallback = null)
    {
        // Create an absolute path
        path = path.StartsWith('/') ? path : '/' + path;
        return Create("https://" + site.SiteUrl + "/" + path, expires, fallback);
    }

    public async Task<(string shortUrl, string qrCode)> Create(string destination, DateTime? expires = null, string fallback = null)
    {
        // Insert the entry in the database. This creates a reference
        // that can be used to create the url and the QR code
        var reference = await Insert(destination, expires, fallback);

        // Create the url and qrCode from the options
        var url = _options.UrlFormat.Replace("{ref}", reference);
        var qrImage = Generator.GetQrImage(url);
        return (url, qrImage.Image);
    }

    public async Task<string> GetDestination(string reference)
    {
        var shortUrl = await mediator.Send(new GetShortUrlByIdRequest(reference));
        if (shortUrl == null) return null;
        if (shortUrl.IsExpired) return shortUrl.Fallback;
        return shortUrl.Destination;
    }

    private async Task<string> Insert(string destination, DateTime? expires, string fallback, int retries = 0)
    {
        try
        {
            var random = RandomNumberGenerator.Create();
            var reference = random.GetAlphaNumericString(8);
            var insert = new ShortUrl
            {
                Destination = destination,
                Expires = expires,
                Fallback = fallback,
                Id = reference
            };
            await mediator.Send(new InsertShortUrlRequest(insert));
            return reference;
        }
        catch (Exception)
        {
            if (retries == 50) throw;
            return await Insert(destination, expires, fallback, retries + 1);
        }
    }
}