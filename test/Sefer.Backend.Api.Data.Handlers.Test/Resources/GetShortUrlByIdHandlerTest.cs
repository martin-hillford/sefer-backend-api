namespace Sefer.Backend.Api.Data.Handlers.Test.Resources;

[TestClass]
public class GetShortUrlByIdHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Test_Handle_NotFound_Empty()
    {
        var provider = GetServiceProvider();
        var handler = new GetShortUrlByIdHandler(provider.Object);
        var request = new GetShortUrlByIdRequest("not-to-be-found");

        var result = await handler.Handle(request, CancellationToken.None);
        
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task Test_Handle()
    {
        var shortUrl = PrepareContext();
        var provider = GetServiceProvider();
        var handler = new GetShortUrlByIdHandler(provider.Object);
        var request = new GetShortUrlByIdRequest(shortUrl.Id);

        var found = await handler.Handle(request, CancellationToken.None);

        found.Id.Should().Be(shortUrl.Id);
        found.Destination.Should().Be(shortUrl.Destination);
        found.Fallback.Should().Be(shortUrl.Fallback);
        found.Expires.Should().Be(shortUrl.Expires);
    }

    [TestMethod]
    public async Task Test_Handle_NotFound()
    {
        var shortUrl = PrepareContext();
        var provider = GetServiceProvider();
        var handler = new GetShortUrlByIdHandler(provider.Object);
        var request = new GetShortUrlByIdRequest(shortUrl.Id + "_not");

        var result = await handler.Handle(request, CancellationToken.None);
        
        Assert.IsNull(result);
    }

    private ShortUrl PrepareContext()
    {
        var expires = DateTime.UtcNow.AddDays(1);
        var shortUrl = new ShortUrl { Id = "tsd1", Destination = "des", Fallback = "fallback", Expires = expires };
        var context = GetDataContext();
        context.ShortUrls.Add(shortUrl);
        context.SaveChanges();
        return shortUrl;
    }
}