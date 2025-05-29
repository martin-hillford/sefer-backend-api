namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetPublishedContentPageByPermalinkHandlerTest : ContentPageHandlerTest
{
    [TestMethod]
    public async Task Handle_PermalinkFound()
    {
        var context = GetDataContext();
        var first = await context.ContentPages.FirstOrDefaultAsync(b => b.IsPublished);

        Assert.IsNotNull(first);

        var request = new GetPublishedContentPageByPermalinkRequest(first.Permalink, null);
        var handler = new GetPublishedContentPageByPermalinkHandler(GetServiceProvider().Object);
        var contentPage = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(contentPage);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    [DataRow("Permalink")]
    public async Task Handle_PermalinkNotFound(string permalink)
    {
        var request = new GetPublishedContentPageByPermalinkRequest(permalink, null);
        var handler = new GetPublishedContentPageByPermalinkHandler(GetServiceProvider().Object);
        var contentPage = await handler.Handle(request, CancellationToken.None);

        Assert.IsNull(contentPage);
    }
}