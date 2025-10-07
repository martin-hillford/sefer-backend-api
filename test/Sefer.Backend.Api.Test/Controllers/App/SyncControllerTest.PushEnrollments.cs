using System.Diagnostics.CodeAnalysis;
using Sefer.Backend.Api.Controllers.App;
using Sefer.Backend.Api.Data.Models.Courses;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Requests.Enrollments;
using Sefer.Backend.Api.Models.App;
using Sefer.Backend.Api.Views.App;

namespace Sefer.Backend.Api.Test.Controllers.App;

[TestClass]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class SyncControllerTest
{
    [TestMethod]
    public async Task PushEnrollments_UserIsNull()
    {
        var local = new List<LocalEnrollment>();
        var server = new List<Enrollment>();
        var (_, result) = await PushEnrollments(null, local, server);
        
        Assert.IsInstanceOfType<ForbidResult>(result);
    }
    
    [TestMethod]
    public async Task PushEnrollments_UserIsMentor()
    {
        var mentor = new User { Id = 1, Name = "Mentor", Role = UserRoles.Mentor };
        var local = new List<LocalEnrollment>();
        var server = new List<Enrollment>();
        var (_, result) = await PushEnrollments(mentor, local, server);
        
        Assert.IsInstanceOfType<ForbidResult>(result);
    }

    [TestMethod]
    public async Task PushEnrollments_NoLocalEnrollmentsToSync()
    {
        var student = new User { Id = 1, Name = "Mentor", Role = UserRoles.Student };
        var locals = new List<LocalEnrollment>();
        var servers = new List<Enrollment>();
        var (_, result) = await PushEnrollments(student, locals, servers);
        var pushResult = (result as ObjectResult)?.Value as SyncView<PushResult>; 
        
        Assert.IsInstanceOfType<OkObjectResult>(result);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SyncView<PushResult>>(pushResult);
        Assert.IsEmpty(pushResult.Data);
    }

    [TestMethod]
    public async Task PushEnrollments_NewLocalEnrollment()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var local = new LocalEnrollment { LocalId = 13, CourseId = 19, Grade = 8, UserId = student.Id };
        var locals = new List<LocalEnrollment> { local };
        var servers = new List<Enrollment>();
        
        await PushEnrollments_AssertInserted(student, locals, servers);
    }
    
    [TestMethod]
    public async Task PushEnrollments_LocalEnrollmentNotMatchingCourse()
    {
        var student = new User { Id = 11, Name = "Student", Role = UserRoles.Student };
        var local = new LocalEnrollment { LocalId = 13, CourseId = 19, Grade = 8, UserId = student.Id };
        var locals = new List<LocalEnrollment> { local };
        var server = new Enrollment { CourseRevision = new CourseRevision { CourseId = 17 } };
        var servers = new List<Enrollment> { server };
        
        await PushEnrollments_AssertInserted(student, locals, servers);
    }

    [TestMethod]
    public async Task PushEnrollments_LocalEnrollmentNotAhead()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var local = new LocalEnrollment { LocalId = 13, CourseId = 19, Grade = 8, UserId = student.Id };
        var locals = new List<LocalEnrollment> { local };
        var server = new Enrollment { CourseRevision = new CourseRevision { CourseId = 19 }, CreationDate = DateTime.UtcNow };
        var servers = new List<Enrollment> { server };
        
        var (provider, result) = await PushEnrollments(student, locals, servers);
        PushEnrollments_AssertUpdate(provider, result, null, false);
    }
    
    [TestMethod]
    public async Task PushEnrollments_LocalEnrollmentAhead()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var local = new LocalEnrollment { LocalId = 13, CourseId = 19, Grade = 8, UserId = student.Id, IsCourseCompleted = true};
        var locals = new List<LocalEnrollment> { local };
        var server = new Enrollment { Id = 17, CourseRevision = new CourseRevision { CourseId = 19 }, CreationDate = DateTime.UtcNow, IsCourseCompleted = false};
        var servers = new List<Enrollment> { server };
        
        var (provider, result) = await PushEnrollments(student, locals, servers);
        PushEnrollments_AssertUpdate(provider, result, "17", true);
    }
    
    [TestMethod]
    public async Task PushEnrollments_MultipleEnrollmentsLocalNotAhead()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var local = new LocalEnrollment { LocalId = 13, CourseId = 19, Grade = 8, UserId = student.Id};
        var locals = new List<LocalEnrollment> { local };
        var servers = new List<Enrollment>
        {
            new () { Id = 17, CourseRevision = new CourseRevision { CourseId = 19 }, CreationDate = DateTime.UtcNow, IsCourseCompleted = true},
            new () { Id = 23, CourseRevision = new CourseRevision { CourseId = 19 }, CreationDate = DateTime.MaxValue },
        };
        
        var (provider, result) = await PushEnrollments(student, locals, servers);
        PushEnrollments_AssertUpdate(provider, result, null, false);
    }
    
    [TestMethod]
    public async Task PushEnrollments_MultipleEnrollmentsLocalAhead()
    {
        var student = new User { Id = 1, Name = "Student", Role = UserRoles.Student };
        var local = new LocalEnrollment { LocalId = 13, CourseId = 19, Grade = 8, UserId = student.Id, IsCourseCompleted = true};
        var locals = new List<LocalEnrollment> { local };
        var servers = new List<Enrollment>
        {
            new () { Id = 23, CourseRevision = new CourseRevision { CourseId = 19 }, CreationDate = DateTime.MaxValue },
            new () { Id = 17, CourseRevision = new CourseRevision { CourseId = 19 }, CreationDate = DateTime.UtcNow, IsCourseCompleted = true},
            
        };
        
        var (provider, result) = await PushEnrollments(student, locals, servers);
        PushEnrollments_AssertUpdate(provider, result, "23", true);
    }
    
    private static async Task PushEnrollments_AssertInserted(User student, List<LocalEnrollment> locals, List<Enrollment> servers)
    {
        var (provider, result) = await PushEnrollments(student, locals, servers);
        
        var pushResult = (result as ObjectResult)?.Value as SyncView<PushResult>;
        Assert.IsInstanceOfType<OkObjectResult>(result);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SyncView<PushResult>>(pushResult);
        Assert.HasCount(1, pushResult.Data);
        Assert.AreEqual("0", pushResult.Data.First().Id);
        Assert.AreEqual(13, pushResult.Data.First().LocalId);

        provider.Mediator.VerifySend<AddEnrollmentRequest, bool>(Times.Once());
        provider.Mediator.VerifySend<UpdateEnrollmentRequest, bool>(Times.Never());     
    }

    private static void PushEnrollments_AssertUpdate(MockedServiceProvider provider, IActionResult result, string serverId, bool updated)
    {
        var pushResult = (result as ObjectResult)?.Value as SyncView<PushResult>;
        Assert.IsInstanceOfType<OkObjectResult>(result);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<SyncView<PushResult>>(pushResult);
        Assert.HasCount(1, pushResult.Data);
        Assert.AreEqual(serverId, pushResult.Data.First().Id);
        Assert.AreEqual(13, pushResult.Data.First().LocalId);

        provider.Mediator.VerifySend<AddEnrollmentRequest, bool>(Times.Never());
        provider.Mediator.VerifySend<UpdateEnrollmentRequest, bool>(updated ? Times.Once() : Times.Never());   
    }
    
    private static async Task<(MockedServiceProvider, IActionResult)> PushEnrollments(User user, List<LocalEnrollment> local, List<Enrollment> server)
    {
        var provider = MockedServiceProvider.Create();
        if(user != null) provider.SetupUser(user);
        if (server != null) provider.AddRequestResult<GetEnrollmentsOfStudentRequest, List<Enrollment>>(server);
        var controller = new SyncController(provider.Object);
        return (provider, await controller.PushEnrollments(local));
    }
}