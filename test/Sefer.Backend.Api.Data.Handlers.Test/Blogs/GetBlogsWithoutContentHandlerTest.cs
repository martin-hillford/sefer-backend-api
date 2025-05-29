namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class GetBlogsWithoutContentHandlerTest : BlogHandlerTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetBlogsWithoutContentRequest();
        var handler = new GetBlogsWithoutContentHandler(GetServiceProvider().Object);
        var blogs = await handler.Handle(request, CancellationToken.None);

        var context = GetDataContext();
        var count = await context.Blogs.CountAsync();
        Assert.AreEqual(count, blogs.Count);
    }
}