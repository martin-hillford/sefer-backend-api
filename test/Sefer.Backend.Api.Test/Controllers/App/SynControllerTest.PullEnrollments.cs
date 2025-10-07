using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Controllers.App;
using Sefer.Backend.Api.Data.Models.Courses;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Requests.Enrollments;
using Sefer.Backend.Api.Support.Extensions;
using Sefer.Backend.Api.Views.App;

namespace Sefer.Backend.Api.Test.Controllers.App;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class SyncControllerTest
{
    [TestMethod]
    public async Task PullEnrollments_UserIsNull()
    {
        var (_, result) = await PullEnrollments(null, []);
        Assert.IsInstanceOfType<ForbidResult>(result);
    }
    
    [TestMethod]
    public async Task PullEnrollments_UserIsMentor()
    {
        var mentor = new User { Id = 1, Name = "Mentor", Role = UserRoles.Mentor };
        var (_, result) = await PullEnrollments(mentor, []);
        Assert.IsInstanceOfType<ForbidResult>(result);
    }
    
    [TestMethod]
    public async Task PullEnrollments_NoDateTime()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var (provider, result) = await PullEnrollments(student, []);
        
        provider.Mediator.Verify(m => m.Send(
            It.Is<GetEnrollmentsOfStudentRequest>(r =>
                r.UserId == student.Id &&
                r.Top == null &&
                r.Extensive == false &&
                r.Start == null
            ), 
            It.IsAny<CancellationToken>()), 
            Times.Once
        );
        
        Assert.IsInstanceOfType<OkObjectResult>(result);
    }
    
    [TestMethod]
    public async Task PullEnrollments_WithDateTime()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var (provider, result) = await PullEnrollments(student, [], 1759822477);
        
        provider.Mediator.Verify(m => m.Send(
                It.Is<GetEnrollmentsOfStudentRequest>(r =>
                    r.UserId == student.Id &&
                    r.Top == null &&
                    r.Extensive == false &&
                    r.Start == DateTimeOffset.FromUnixTimeSeconds(1759822477).DateTime
                ), 
                It.IsAny<CancellationToken>()), 
            Times.Once
        );
        
        Assert.IsInstanceOfType<OkObjectResult>(result);
    }

    [TestMethod]
    public async Task PullEnrollments_CorrectViewRendering()
    {
        var student = new User { Id = 19, Name = "Student", Role = UserRoles.Student };
        var enrollments = new List<Enrollment>
        {
            new()
            {
                ClosureDate = DateTime.MaxValue, Grade = 8, Id = 13, IsCourseCompleted = false, CourseRevisionId = 10,
                CourseRevision = new CourseRevision { CourseId = 17 }, CreationDate = DateTime.MinValue,
                ModificationDate = DateTime.UnixEpoch, StudentId = 19
            },
            new()
            {
                ClosureDate = DateTime.MaxValue, Grade = 6, Id = 37, IsCourseCompleted = true, CourseRevisionId = 43,
                CourseRevision = new CourseRevision { CourseId = 47 }, CreationDate = DateTime.MinValue,
                ModificationDate = DateTime.UnixEpoch, StudentId = 19
            }
        };
        var (_, result) = await PullEnrollments(student, enrollments, 1759822477);
        
        
        Assert.IsInstanceOfType<OkObjectResult>(result);
        Assert.IsInstanceOfType<SyncView<EnrollmentView>>((result as ObjectResult)?.Value);
        
        var syncView = ((ObjectResult)result).Value as SyncView<EnrollmentView>;
        Assert.IsNotNull(syncView);
        Assert.HasCount(2, syncView.Data);
        
        Assert.AreEqual(8, syncView.Data[0].Grade);
        Assert.AreEqual(DateTime.MaxValue.ToUnixTime(), syncView.Data[0].ClosureDate);
        Assert.AreEqual(13, syncView.Data[0].Id);
        Assert.AreEqual(0, syncView.Data[0].IsCourseCompleted);
        Assert.AreEqual(10, syncView.Data[0].CourseRevisionId);
        Assert.AreEqual(17, syncView.Data[0].CourseId);
        Assert.AreEqual(DateTime.MinValue.ToUnixTime(), syncView.Data[0].CreationDate);
        Assert.AreEqual(DateTime.UnixEpoch.ToUnixTime(), syncView.Data[0].ModificationDate);
        Assert.AreEqual(19, syncView.Data[0].StudentId);
        
    }
    
    private static async Task<(MockedServiceProvider, IActionResult)> PullEnrollments(User user, List<Enrollment> enrollments, long? prevSyncTime = null)
    {
        var provider = MockedServiceProvider.Create();
        if(user != null) provider.SetupUser(user);
        if (enrollments != null) provider.AddRequestResult<GetEnrollmentsOfStudentRequest, List<Enrollment>>(enrollments);
        var controller = new SyncController(provider.Object);
        return (provider, await controller.PullEnrollments(prevSyncTime));
    }
}