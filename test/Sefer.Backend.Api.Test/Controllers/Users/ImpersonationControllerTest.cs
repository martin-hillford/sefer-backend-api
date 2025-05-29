using Sefer.Backend.Api.Controllers.Users;
using Sefer.Backend.Api.Models.Users;

namespace Sefer.Backend.Api.Test.Controllers.Users;

[TestClass]
public class ImpersonationControllerTest
{
    [TestMethod]
    public async Task SetImpersonation_BodyNull()
    {
        var serviceProvider = MockedServiceProvider.Create();
        var controller = new ImpersonationController(serviceProvider.Object);
        var response = await controller.SetImpersonation(null);
        Assert.IsNotNull(response as BadRequestResult);
    }

    [TestMethod]
    public async Task SetImpersonation_UserNull()
    {
        var serviceProvider = MockedServiceProvider.Create();
        var controller = new ImpersonationController(serviceProvider.Object);
        var body = new ImpersonationPostModel { AllowImpersonation = false };
        var response = await controller.SetImpersonation(body);
        Assert.IsNotNull(response as UnauthorizedResult);
    }

    [TestMethod]
    public async Task SetImpersonation()
    {
        var user = new User { AllowImpersonation = true, Id = 13 };
        var serviceProvider = MockedServiceProvider.Create().SetupUser(user);
        var controller = new ImpersonationController(serviceProvider.Object);
        var body = new ImpersonationPostModel { AllowImpersonation = false };
        var response = await controller.SetImpersonation(body);
        Assert.IsNotNull(response as OkResult);
        Assert.IsFalse(user.AllowImpersonation);
    }
}