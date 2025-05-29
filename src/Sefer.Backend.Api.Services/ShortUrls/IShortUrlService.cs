// ReSharper disable UnusedMember.Global
namespace Sefer.Backend.Api.Services.ShortUrls;

public interface IShortUrlService
{
    Task<(string shortUrl, string qrCode)> Create(ISite site, string path, DateTime? expires = null, string fallback = null);

    Task<(string shortUrl, string qrCode)> Create(string destination, DateTime? expires = null, string fallback = null);

    Task<string> GetDestination(string reference);
}