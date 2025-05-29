namespace Sefer.Backend.Api.Data.Handlers.Test.Blogs;

[TestClass]
public class GetPublishedBlogsHandlerTest : BlogHandlerTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetPublishedBlogsRequest();
        var handler = new GetPublishedBlogsHandler(GetServiceProvider().Object);
        var blogs = await handler.Handle(request, CancellationToken.None);
        
        Assert.AreEqual(2, blogs.Count);
        
        Assert.AreEqual("test1", blogs[0].Name);
        Assert.AreEqual("test2", blogs[1].Name);
    }
    
    [TestMethod]
    public async Task Handle_WithTake()
    {
        var request = new GetPublishedBlogsRequest(1);
        var handler = new GetPublishedBlogsHandler(GetServiceProvider().Object);
        var blogs = await handler.Handle(request, CancellationToken.None);
        
        Assert.AreEqual(1, blogs.Count);
        
        Assert.AreEqual("test1", blogs[0].Name);
    }
}