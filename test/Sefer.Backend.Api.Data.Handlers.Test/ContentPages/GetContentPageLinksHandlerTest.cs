namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetContentPageLinksHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var pageA = new ContentPage { Content = "testA", Permalink = "A", Name = "testA", IsPublished = true, SequenceId = 2 };
        var pageB = new ContentPage { Content = "testB", Permalink = "B", Name = "testB", IsPublished = true, SequenceId = 1 };

        var context = GetDataContext();
        await context.AddAsync(pageA);
        await context.AddAsync(pageB);
        await context.SaveChangesAsync();

        var request = new GetContentPageLinksRequest();
        var handler = new GetContentPageLinksHandler(GetServiceProvider().Object);
        var pages = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(2, pages.Count);

        AssertAreEqual(pageA, pages.Last());
        AssertAreEqual(pageB, pages.First());
    }

    private static void AssertAreEqual(ContentPage page, ContentPageLink link)
    {
        Assert.AreEqual(page.Id, link.Id);
        Assert.AreEqual(page.Name, link.Name);
        Assert.AreEqual(page.Permalink, link.Permalink);
        Assert.AreEqual(page.Type, link.Type);
        Assert.AreEqual(page.IsPublished, link.IsPublished);
    }
}