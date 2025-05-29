namespace Sefer.Backend.Api.Data.Handlers.Test.ContentPages;

[TestClass]
public class IsContentPageNameUniqueHandlerTest : ContentPageHandlerTest
{
    [TestMethod]
    [DataRow("test1", false)]
    [DataRow("testA", true)]
    [DataRow(null, true)]
    [DataRow(" ", true)]
    [DataRow("", true)]
    public async Task Handle(string name, bool expectUnique)
    {
        var request = new IsContentPageNameUniqueRequest(null, name);
        var handler = new IsContentPageNameUniqueHandler(GetServiceProvider().Object);
        var unique = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expectUnique, unique);
    }

    [TestMethod]
    public async Task Handle_WithSameContentPage()
    {
        var context = GetDataContext();
        var first = await context.ContentPages.FirstOrDefaultAsync(b => b.IsPublished);
        Assert.IsNotNull(first);

        var request = new IsContentPageNameUniqueRequest(first.Id, first.Name);
        var handler = new IsContentPageNameUniqueHandler(GetServiceProvider().Object);
        var unique = await handler.Handle(request, CancellationToken.None);
        Assert.IsTrue(unique);
    }
}