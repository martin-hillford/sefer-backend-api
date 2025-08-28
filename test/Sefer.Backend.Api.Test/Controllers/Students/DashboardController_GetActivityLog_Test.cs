// ReSharper disable InconsistentNaming
namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public partial class DashboardControllerTest
{
    [TestMethod]
    public async Task GetActivityLog_NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActivityLog();
        
        Assert.IsInstanceOfType(response.Result, typeof(ForbidResult));
        Assert.IsNull(response.Value);
    }
    
    [TestMethod]
    public async Task GetActivityLog_Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActivityLog();

        Assert.IsInstanceOfType(response.Result, typeof(ForbidResult));
        Assert.IsNull(response.Value);
    }
}



