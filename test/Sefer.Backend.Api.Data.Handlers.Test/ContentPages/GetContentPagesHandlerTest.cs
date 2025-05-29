namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetContentPagesHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        context.Add(new ContentPage { Content = "z-test", Name = "z-test", Type = ContentPageType.IndividualPage, SequenceId = 2 });
        context.Add(new ContentPage { Content = "test", Name = "test", Type = ContentPageType.IndividualPage, SequenceId = 1 });
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetContentPagesRequest();
        var handler = new GetContentPagesHandler(GetServiceProvider().Object);
        var pages = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(2, pages.Count);
        Assert.AreEqual("test", pages[0].Name);
        Assert.AreEqual("z-test", pages[1].Name);
    }
}