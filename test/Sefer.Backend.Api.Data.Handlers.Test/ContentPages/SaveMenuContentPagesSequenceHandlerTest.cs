namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class SaveMenuContentPagesSequenceHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        var pages = new List<ContentPage>
        {
            new () {Name = "test1", Permalink = "test1", Content = "content", IsPublished = true, Type = ContentPageType.MenuPage, SequenceId = 1},
            new () {Name = "test2", Permalink = "test2", Content = "content", IsPublished = false, Type = ContentPageType.MenuPage, SequenceId = 2},
            new () {Name = "test3", Permalink = "test3", Content = "content", Type = ContentPageType.IndividualPage },
        };
        await context.ContentPages.AddRangeAsync(pages);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle_Null()
    {
        var result = await Handle(null);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_EmptyCollection()
    {
        var result = await Handle(new List<ContentPage>());
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_PageMissing()
    {
        var context = GetDataContext();
        var pages = await context.ContentPages.Where(c => c.Name == "test1").ToListAsync();

        var result = await Handle(pages);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_WrongPageType()
    {
        var context = GetDataContext();
        var pages = await context.ContentPages.Where(c => c.Name != "test2").ToListAsync();

        var result = await Handle(pages);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_SameSequence()
    {
        var save = await GetMenuPages();

        var result = await Handle(save);
        Assert.IsTrue(result);

        var pages = await GetMenuPages();

        Assert.AreEqual(2, pages.Count);
        Assert.AreEqual("test1", pages.First().Name);
        Assert.AreEqual("test2", pages.Last().Name);
    }

    [TestMethod]
    public async Task Handle()
    {
        var save = await GetMenuPages();

        var result = await Handle([save.Last(), save.First()]);
        Assert.IsTrue(result);

        var pages = await GetMenuPages();

        Assert.AreEqual(2, pages.Count);
        Assert.AreEqual("test2", pages.First().Name);
        Assert.AreEqual("test1", pages.Last().Name);
    }

    private async Task<bool> Handle(List<ContentPage>? contentPages)
    {
        var request = new SaveMenuContentPagesSequenceRequest(contentPages);
        var handler = new SaveMenuContentPagesSequenceHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }

    private async Task<List<ContentPage>> GetMenuPages()
    {
        var context = GetDataContext();
        return await context.ContentPages
            .Where(c => c.Type == ContentPageType.MenuPage)
            .OrderBy(c => c.SequenceId)
            .ToListAsync();
    }
}