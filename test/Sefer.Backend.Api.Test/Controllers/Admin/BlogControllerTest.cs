using Sefer.Backend.Api.Data.Models.Resources;
using BlogController = Sefer.Backend.Api.Controllers.Admin.BlogController;

namespace Sefer.Backend.Api.Test.Controllers.Admin;

[TestClass]
public class BlogControllerTest
{
    [TestMethod]
    public async Task NoBlogsInSystem()
    {
        var services = new IntegrationServices();
        var provider = services.BuildServiceProvider();
        var controller = new BlogController(provider);

        var actionResult = await controller.GetBlogs();
        var okObjectResult = actionResult.Result as OkObjectResult;
        var blogs = okObjectResult?.Value as List<BlogBase>;
        
        Assert.IsNotNull(actionResult); Assert.IsNotNull(okObjectResult); Assert.IsNotNull(blogs);
        Assert.IsInstanceOfType<ActionResult<List<BlogBase>>>(actionResult);
        Assert.IsInstanceOfType<List<BlogBase>>(blogs);
        Assert.AreEqual(0, blogs.Count);
    }
}