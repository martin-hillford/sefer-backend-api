// ReSharper disable InconsistentNaming
namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public class DashboardController_GetActivityLog_Test : AbstractControllerTest
{
    [TestMethod]
    public async Task NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActivityLog();
        
        Assert.IsInstanceOfType(response.Result, typeof(ForbidResult));
        Assert.IsNull(response.Value);
    }
    
    [TestMethod]
    public async Task Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActivityLog();

        Assert.IsInstanceOfType(response.Result, typeof(ForbidResult));
        Assert.IsNull(response.Value);
    }
}



