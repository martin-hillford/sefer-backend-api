namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class GetPublishedBlogByPermalinkHandlerTest : BlogHandlerTest
{
    [TestMethod]
    public async Task Handle_PermalinkFound()
    {
        var context = GetDataContext();
        var first = await context.Blogs.FirstOrDefaultAsync(b => b.IsPublished);

        Assert.IsNotNull(first);

        var request = new GetPublishedBlogByPermalinkRequest(first.Permalink);
        var handler = new GetPublishedBlogByPermalinkHandler(GetServiceProvider().Object);
        var blog = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(blog?.Author);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    [DataRow("Permalink")]
    public async Task Handle_PermalinkNotFound(string permalink)
    {
        var request = new GetPublishedBlogByPermalinkRequest(permalink);
        var handler = new GetPublishedBlogByPermalinkHandler(GetServiceProvider().Object);
        var blog = await handler.Handle(request, CancellationToken.None);

        Assert.IsNull(blog);
    }
}