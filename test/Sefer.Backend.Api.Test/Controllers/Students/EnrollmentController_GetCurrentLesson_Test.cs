// ReSharper disable InconsistentNaming

using Microsoft.Extensions.DependencyInjection;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Models.Settings;
using Sefer.Backend.Api.Data.Requests.Enrollments;
using Sefer.Backend.Api.Data.Requests.Settings;

namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public class EnrollmentController_GetCurrentLesson_Test : AbstractControllerTest
{
    [TestMethod]
    public async Task NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new Api.Controllers.Student.EnrollmentController(mocked.Object);
        Assert.IsInstanceOfType(await controller.GetCurrentLesson(), typeof(ForbidResult));
        Assert.IsInstanceOfType(await controller.GetCurrentLesson(13), typeof(ForbidResult));
    } 
    
    [TestMethod]
    public async Task Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new Api.Controllers.Student.EnrollmentController(mocked.Object);
        Assert.IsInstanceOfType(await controller.GetCurrentLesson(), typeof(ForbidResult));
        Assert.IsInstanceOfType(await controller.GetCurrentLesson(13), typeof(ForbidResult));
    }
    
    [TestMethod]
    public async Task MultipleActiveEnrollmentsEnabled()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, true);
        var controller = new Api.Controllers.Student.EnrollmentController(mocked.Object);

        var result = await controller.GetCurrentLesson() as StatusCodeResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(418, result.StatusCode);
    }
    
    [TestMethod]
    public async Task NotFound()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, false);
        var controller = new Api.Controllers.Student.EnrollmentController(mocked.Object);
        mocked.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([]);

        var result = await controller.GetCurrentLesson();

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }
    
    [TestMethod]
    public async Task NoLesson()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, false);
        var controller = new Api.Controllers.Student.EnrollmentController(mocked.Object);
        var enrollment = new Enrollment { Id = 13 };
        mocked.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([enrollment]);

        var result = await controller.GetCurrentLesson();
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task NoEnrollment()
    {
        var services = new IntegrationServices();
        services.CreateCourse("course");
        services.CreateStudentAndSetAsCurrent();
        
        var controller = new Api.Controllers.Student.EnrollmentController(services.BuildServiceProvider());
        
        var result = await controller.GetCurrentLesson(29);
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }
    
    [TestMethod]
    public async Task Ok()
    {
        var services = new IntegrationServices();
        var (revision, _ ) = services.CreateCourse("course");
        var student = services.CreateStudentAndSetAsCurrent();
        var enrollment = services.AddEnrollment(student, revision, null);
        
        var controller = new Api.Controllers.Student.EnrollmentController(services.BuildServiceProvider());
        
        var result = await controller.GetCurrentLesson(enrollment.Id);
        Assert.IsInstanceOfType<JsonResult>(result);
    }
    
    private static MockedServiceProvider Create(User user, bool allowMultipleActiveEnrollments)
    {
        var settings = new Settings { AllowMultipleActiveEnrollments = allowMultipleActiveEnrollments };
        var mocked = GetServiceProvider(user);
        mocked.AddRequestResult<GetSettingsRequest, Settings>(settings);
        return mocked;
    }
}