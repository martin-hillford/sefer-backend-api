namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class GetPublishedSpecialPageByTypeHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        var pages = new List<ContentPage>
        {
            new () {Name = "test1", Permalink = "test1", Content = "content", IsPublished = true, Type = ContentPageType.MenuPage, SequenceId = 1},
            new () {Name = "test3", Permalink = "test3", Content = "content", IsPublished = true, Type = ContentPageType.IndividualPage },
            new () {Name = "test3", Permalink = "test3", Content = "content", IsPublished = true, Type = ContentPageType.PrivacyStatementPage },
            new () {Name = "test3", Permalink = "test3", Content = "content", IsPublished = true, Type = ContentPageType.UsageTermsPage },
            new () {Name = "test3", Permalink = "test3", Content = "content", Type = ContentPageType.HomePage },
        };
        await context.ContentPages.AddRangeAsync(pages);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    [DataRow(ContentPageType.IndividualPage, true)]
    [DataRow(ContentPageType.MenuPage, true)]
    [DataRow(ContentPageType.PrivacyStatementPage, false)]
    [DataRow(ContentPageType.UsageTermsPage, false)]
    [DataRow(ContentPageType.HomePage, true)]
    public async Task HandleTest(ContentPageType type, bool expectNull)
    {
        var context = GetDataContext();
        var count = await context.ContentPages.CountAsync();
        Assert.AreEqual(5, count);

        var request = new GetPublishedSpecialPageByTypeRequest(type, null);
        var handler = new GetPublishedSpecialPageByTypeHandler(GetServiceProvider().Object);
        var page = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(expectNull, page == null);
    }
}