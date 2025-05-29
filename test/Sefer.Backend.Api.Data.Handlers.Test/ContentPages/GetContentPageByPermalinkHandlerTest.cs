namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetContentPageByPermalinkHandlerTest : ContentPageHandlerTest
{
    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var first = await context.ContentPages.FirstOrDefaultAsync(b => b.IsPublished);

        Assert.IsNotNull(first);

        var request = new GetContentPageByPermalinkRequest(first.Permalink);
        var handler = new GetContentPageByPermalinkHandler(GetServiceProvider().Object);
        var page = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(page);
        Assert.AreEqual(first.Id, page.Id);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    [DataRow("Permalink")]
    public async Task Handle_PermalinkNotFound(string permalink)
    {
        var request = new GetContentPageByPermalinkRequest(permalink);
        var handler = new GetContentPageByPermalinkHandler(GetServiceProvider().Object);
        var contentPage = await handler.Handle(request, CancellationToken.None);

        Assert.IsNull(contentPage);
    }
}