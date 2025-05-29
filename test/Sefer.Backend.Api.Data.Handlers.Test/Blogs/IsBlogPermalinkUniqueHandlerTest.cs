namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class IsBlogPermalinkUniqueHandlerTest : BlogHandlerTest
{
    [TestMethod]
    [DataRow("test1", false)]
    [DataRow("testA", true)]
    [DataRow(null, true)]
    [DataRow(" ", true)]
    [DataRow("", true)]
    public async Task Handle(string permalink, bool expectUnique)
    {
        var request = new IsBlogPermalinkUniqueRequest(null, permalink);
        var handler = new IsBlogPermalinkUniqueHandler(GetServiceProvider().Object);
        var unique = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expectUnique, unique);
    }

    [TestMethod]
    public async Task Handle_WithSameBlog()
    {
        var context = GetDataContext();
        var first = await context.Blogs.FirstOrDefaultAsync(b => b.IsPublished);
        Assert.IsNotNull(first);

        var request = new IsBlogPermalinkUniqueRequest(first.Id, first.Permalink);
        var handler = new IsBlogPermalinkUniqueHandler(GetServiceProvider().Object);
        var unique = await handler.Handle(request, CancellationToken.None);
        Assert.IsTrue(unique);
    }
}