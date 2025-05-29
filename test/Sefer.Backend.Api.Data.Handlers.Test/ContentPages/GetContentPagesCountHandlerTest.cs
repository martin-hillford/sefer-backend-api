namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetContentPagesCountHandlerTest : ContentPageHandlerTest
{
    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var count = await context.ContentPages.CountAsync();

        var handler = new GetContentPagesCountHandler(GetServiceProvider().Object);
        var request = new GetContentPagesCountRequest();

        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(count, result);
    }
}