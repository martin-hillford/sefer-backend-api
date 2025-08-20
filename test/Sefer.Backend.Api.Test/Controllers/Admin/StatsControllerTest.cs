using Microsoft.Extensions.Configuration;
using Sefer.Backend.Api.Controllers.Admin;

namespace Sefer.Backend.Api.Test.Controllers.Admin;

[TestClass]
public class StatsControllerTest
{
    [TestMethod]
    public void GetStats_NoConfig()
    {
        var mocked = new MockedServiceProvider();
        var controller = new StatsController(null, mocked.Object);
        var result = controller.GetStats("dashboard?timezone=-90");
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }
    
    [TestMethod]
    public void GetStats_WithConfig()
    {
        var mocked = new MockedServiceProvider();
        var config = new Mock<IConfiguration>();
        var section = new Mock<IConfigurationSection>();
        section.Setup(s => s.Value).Returns("https://stats.example.com");        
        config.Setup(c => c.GetSection("StatsApi")).Returns(section.Object);
        
        var controller = new StatsController(config.Object, mocked.Object);
        var result = controller.GetStats("dashboard?timezone=-90");

        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<RedirectResult>(result);
        Assert.AreEqual("https://stats.example.com/dashboard?timezone=-90", (result as RedirectResult)?.Url);;
    }    
}