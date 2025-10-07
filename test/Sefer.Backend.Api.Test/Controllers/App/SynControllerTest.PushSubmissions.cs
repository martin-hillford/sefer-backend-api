using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Controllers.App;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Requests.Enrollments;
using Sefer.Backend.Api.Models.App;

namespace Sefer.Backend.Api.Test.Controllers.App;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class SyncControllerTest
{
    [TestMethod]
    public async Task PushSubmissions_UserIsNull()
    {
        var local = new List<LocalSubmission>();
        var (_, result) = await PushSubmissions(null, local);
        
        Assert.IsInstanceOfType<ForbidResult>(result);
    }
    
    [TestMethod]
    public async Task PushSubmissions_UserIsMentor()
    {
        var mentor = new User { Id = 1, Name = "Mentor", Role = UserRoles.Mentor };
        var local = new List<LocalSubmission>();
        var (_, result) = await PushSubmissions(mentor, local);
        
        Assert.IsInstanceOfType<ForbidResult>(result);
    }
    
    private static async Task<(MockedServiceProvider, IActionResult)> PushSubmissions(User user, List<LocalSubmission> local)
    {
        var provider = MockedServiceProvider.Create();
        if(user != null) provider.SetupUser(user);
        var controller = new SyncController(provider.Object);
        return (provider, await controller.PushSubmission(local));
    }
}