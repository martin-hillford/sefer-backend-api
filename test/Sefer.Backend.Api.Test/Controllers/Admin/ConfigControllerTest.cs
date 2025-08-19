using Sefer.Backend.Api.Controllers.Admin;
using Sefer.Backend.Api.Data.Models.Settings;
using Sefer.Backend.Api.Data.Requests.Settings;

namespace Sefer.Backend.Api.Test.Controllers.Admin;

[TestClass]
public class ConfigControllerTest
{
    [TestMethod]
    public async Task GetConfig_Ok()
    {
        var settings = new Settings();
        var serviceProvider = MockedServiceProvider.Create()
            .AddRequestResult<GetSettingsRequest, Settings>(settings);
        var passwordService = new Mock<IPasswordService>();
        var controller = new ConfigController(serviceProvider.Object, passwordService.Object);

        var result = await controller.GetConfig() as OkObjectResult;
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task Setup_ModelNull()
    {
        var serviceProvider = MockedServiceProvider.Create();
        var passwordService = new Mock<IPasswordService>();
        var controller = new ConfigController(serviceProvider.Object, passwordService.Object);
        
        var result = await controller.Setup(null) as BadRequestResult;
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task Setup_UsersPresent()
    {
        var model = new RegistrationPostModel();
        var serviceProvider = MockedServiceProvider.Create()
            .AddRequestResult<GetUsersCountRequest, long>(1);
        var passwordService = new Mock<IPasswordService>();
        
        var controller = new ConfigController(serviceProvider.Object, passwordService.Object);
        
        var result = await controller.Setup(model) as ConflictObjectResult;
        Assert.IsNotNull(result);
    }
 
    [TestMethod]
    public async Task Setup_SiteNull()  => await Setup_SiteRegion(null, null);
    
    [TestMethod]
    public async Task Setup_RegionNull()  => await Setup_SiteRegion(new Mock<ISite>(), null);

    [TestMethod]
    public async Task Setup_SiteNotInRegion()
    {
        var site = new Mock<ISite>(); 
        var region = new Mock<IRegion>();
        await Setup_SiteRegion(site, region);
    }
    
    private static async Task Setup_SiteRegion(Mock<ISite> site, Mock<IRegion> region)
    {
        var model = new RegistrationPostModel { Site = "example.com", Region = "us"};
        var controller = CreateController(site, region, false, false);
        
        var result = await controller.Setup(model) as ObjectResult;
        var details = result?.Value as ProblemDetails;
        
        Assert.IsNotNull(result);
        Assert.IsNotNull(details);
        Assert.AreEqual(500, result.StatusCode);
        Assert.AreEqual("Site or region don't exists or site not in region", details.Detail);
    }

    [TestMethod]
    public async Task Setup_UserNotSaved()
    {
        var model = new RegistrationPostModel { Site = "example.com", Region = "us"};
        var site = new Mock<ISite>(); var region = new Mock<IRegion>();
        var controller = CreateController(site, region, true, false);
        
        var result = await controller.Setup(model) as BadRequestResult;
        Assert.IsNotNull(result);
    }
    
    [TestMethod]
    public async Task Setup_Created()
    {
        var model = new RegistrationPostModel { Site = "example.com", Region = "us"};
        var site = new Mock<ISite>(); var region = new Mock<IRegion>();
        var controller = CreateController(site, region, true,true);
        
        var result = await controller.Setup(model) as CreatedResult;
        Assert.IsNotNull(result);
    }

    private static ConfigController CreateController(Mock<ISite> site, Mock<IRegion> region, bool setup, bool addResult)
    {
        if (setup)
        {
            site?.Setup(r => r.RegionId).Returns("nl"); 
            site?.Setup(s => s.Type).Returns(SiteType.Dynamic);    
            region?.Setup(r => r.Id).Returns("nl");    
        }
        
        var serviceProvider = MockedServiceProvider.Create()
            .AddRequestResult<GetUsersCountRequest, long>(0)
            .AddRequestResult<GetSiteByNameRequest, ISite>(site?.Object)
            .AddRequestResult<GetRegionByIdRequest, IRegion>(region?.Object)
            .AddRequestResult<AddUserRequest, bool>(addResult);
        var passwordService = new Mock<IPasswordService>();
        return new ConfigController(serviceProvider.Object, passwordService.Object);  
    }
}