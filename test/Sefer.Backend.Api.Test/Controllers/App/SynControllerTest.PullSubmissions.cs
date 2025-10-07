using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Controllers.App;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Requests.Submissions;

namespace Sefer.Backend.Api.Test.Controllers.App;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class SyncControllerTest
{
    [TestMethod]
    public async Task PullSubmissions_UserIsNull()
    {
        var (_, result) = await PullSubmissions(null, []);
        Assert.IsInstanceOfType<ForbidResult>(result);
    }
    
    [TestMethod]
    public async Task PullSubmissions_UserIsMentor()
    {
        var mentor = new User { Id = 1, Name = "Mentor", Role = UserRoles.Mentor };
        var (_, result) = await PullSubmissions(mentor, []);
        Assert.IsInstanceOfType<ForbidResult>(result);
    }
    
    [TestMethod]
    public async Task PullSubmissions_NoDateTime()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var (provider, result) = await PullSubmissions(student, []);
        
        provider.Mediator.Verify(m => m.Send(
            It.Is<GetSubmissionsByTimeRequest>(r =>
                r.StudentId == student.Id &&
                r.Start == null
            ), 
            It.IsAny<CancellationToken>()), 
            Times.Once
        );
        
        Assert.IsInstanceOfType<OkObjectResult>(result);
    }
    
    [TestMethod]
    public async Task PullSubmissions_WithDateTime()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var (provider, result) = await PullSubmissions(student, [], 1759822477);
        
        provider.Mediator.Verify(m => m.Send(
                It.Is<GetSubmissionsByTimeRequest>(r =>
                    r.StudentId == student.Id &&
                    r.Start == DateTimeOffset.FromUnixTimeSeconds(1759822477).DateTime
                ), 
                It.IsAny<CancellationToken>()), 
            Times.Once
        );
        
        Assert.IsInstanceOfType<OkObjectResult>(result);
    }

    
    private static async Task<(MockedServiceProvider, IActionResult)> PullSubmissions(User user, List<LessonSubmission> submissions, long? prevSyncTime = null)
    {
        var provider = MockedServiceProvider.Create();
        if(user != null) provider.SetupUser(user);
        if (submissions != null) provider.AddRequestResult<GetSubmissionsByTimeRequest, List<LessonSubmission>>(submissions);
        var controller = new SyncController(provider.Object);
        return (provider, await controller.PullSubmissions(prevSyncTime));
    }
}