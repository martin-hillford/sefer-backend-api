namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetPublishedMenuPagesLinksHandlerTest : ContentPageHandlerTest
{
    [TestInitialize]
    public override async Task Init()
    {
        await base.Init();
        var context = GetDataContext();
        var pages = new List<ContentPage>
        {
            new () {Name = "test4", Permalink = "test4", Content = "content", IsPublished = false, Type = ContentPageType.MenuPage},
            new () {Name = "test5", Permalink = "test5", Content = "content", IsPublished = true , Type = ContentPageType.IndividualPage},
            new () {Name = "test6", Permalink = "test6", Content = "content", IsPublished = true, Type = ContentPageType.MenuPage, SequenceId = 4},
        };
        await context.ContentPages.AddRangeAsync(pages);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetPublishedMenuPagesLinksRequest();
        var handler = new GetPublishedMenuPagesLinksHandler(GetServiceProvider().Object);
        var links = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(2, links.Count);
        Assert.AreEqual("test6", links.Last().Name);
        Assert.AreEqual("test1", links.First().Name);
    }
}