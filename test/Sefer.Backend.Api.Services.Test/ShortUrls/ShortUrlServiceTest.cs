using Microsoft.Extensions.Options;

namespace Sefer.Backend.Api.Services.Test.ShortUrls;

[TestClass]
public class ShortUrlServiceTest
{
    [TestMethod]
    public async Task Test_GetDestination_ReferenceNull()
    {
        var service = CreateService(null);

        var destination = await service.GetDestination(null);

        destination.Should().BeNull();
    }

    [TestMethod]
    public async Task Test_GetDestination_Expired()
    {
        var expires = DateTime.UtcNow.AddSeconds(-1);
        var shortUrl = new ShortUrl { Destination = "dest", Fallback = "fallback", Expires = expires };
        var service = CreateService(shortUrl);

        var destination = await service.GetDestination(null);

        destination.Should().Be("fallback");
    }

    [TestMethod]
    public async Task Test_GetDestination()
    {
        var expires = DateTime.UtcNow.AddSeconds(10);
        var shortUrl = new ShortUrl { Destination = "dest", Fallback = "fallback", Expires = expires };
        var service = CreateService(shortUrl);

        var destination = await service.GetDestination(null);

        destination.Should().Be("dest");
    }

    private static ShortUrlService CreateService(ShortUrl? shortUrl)
    {
        var options = Options.Create(new ShortUrlServiceOptions());
        var mediator = new Mock<IMediator>();
        if(shortUrl != null) mediator.SetupRequest<GetShortUrlByIdRequest, ShortUrl>(shortUrl);
        return new ShortUrlService(mediator.Object, options);
    }

    [TestMethod]
    public async Task Test_Create()
    {
        var options = new ShortUrlServiceOptions { UrlFormat = "https://t.nl/{ref}" };
        var mediator = new Mock<IMediator>();
        var service = new ShortUrlService(mediator.Object, Options.Create(options));
        var destination = "https://dest.to";

        var (shortUrl, qrCode) = await service.Create(destination);

        qrCode.Should().NotBeNullOrEmpty();
        shortUrl.Should().NotBeNullOrEmpty();
        shortUrl.StartsWith("https://t.nl/").Should().BeTrue();
    }
}