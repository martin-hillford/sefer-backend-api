namespace Sefer.Backend.Api.Data.Handlers.Test.Resources;

[TestClass]
public class InsertShortUrlHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Test_Handle_NoId()
    {
        var shortUrl = new ShortUrl();

        var provider = GetServiceProvider();
        var handler = new InsertShortUrlHandler(provider.Object);
        var request = new InsertShortUrlRequest(shortUrl);

        var added = await handler.Handle(request, CancellationToken.None);

        added.Should().BeFalse();
        var context = GetDataContext();
        context.ShortUrls.Should().BeEmpty();
    }

    [TestMethod]
    public async Task Test_Handle()
    {
        var expires = DateTime.UtcNow.AddDays(1);
        var shortUrl = new ShortUrl { Id = "tsd1", Destination = "des", Fallback = "fallback", Expires = expires };

        var provider = GetServiceProvider();
        var handler = new InsertShortUrlHandler(provider.Object);
        var request = new InsertShortUrlRequest(shortUrl);

        var added = await handler.Handle(request, CancellationToken.None);

        added.Should().BeTrue();
        var context = GetDataContext();
        context.ShortUrls.Count().Should().Be(1);
        context.ShortUrls.First().Id.Should().Be(shortUrl.Id);
        context.ShortUrls.First().Destination.Should().Be(shortUrl.Destination);
        context.ShortUrls.First().Fallback.Should().Be(shortUrl.Fallback);
        context.ShortUrls.First().Expires.Should().Be(shortUrl.Expires);
    }

    [TestMethod]
    public async Task Test_Handle_SameIdShouldFail()
    {
        var expires = DateTime.UtcNow.AddDays(1);
        var shortUrl = new ShortUrl { Id = "tsd1", Destination = "des", Fallback = "fallback", Expires = expires };

        var provider = GetServiceProvider();
        var handler = new InsertShortUrlHandler(provider.Object);
        var request = new InsertShortUrlRequest(shortUrl);

        var firstInserted = await handler.Handle(request, CancellationToken.None);
        var secondInserted = await handler.Handle(request, CancellationToken.None);

        firstInserted.Should().BeTrue();
        secondInserted.Should().BeFalse();
    }
}