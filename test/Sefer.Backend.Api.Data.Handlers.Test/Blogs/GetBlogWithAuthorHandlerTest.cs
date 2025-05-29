namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class GetBlogWithAuthorHandlerTest : BlogHandlerTest
{
    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var first = await context.Blogs.FirstOrDefaultAsync();

        Assert.IsNotNull(first);

        var request = new GetBlogWithAuthorRequest(first.Id);
        var handler = new GetBlogWithAuthorHandler(GetServiceProvider().Object);
        var blog = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(blog?.Author);
    }
}

