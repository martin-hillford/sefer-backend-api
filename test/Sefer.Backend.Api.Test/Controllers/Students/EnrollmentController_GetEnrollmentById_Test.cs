// ReSharper disable InconsistentNaming

using Microsoft.Extensions.DependencyInjection;

namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public class EnrollmentController_GetEnrollmentById_Test : AbstractControllerTest
{
    [TestMethod]
    public async Task NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new Api.Controllers.Student.EnrollmentController(mocked.Object);
        Assert.IsInstanceOfType(await controller.GetEnrollmentById(13), typeof(ForbidResult));
    } 
    
    [TestMethod]
    public async Task Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new Api.Controllers.Student.EnrollmentController(mocked.Object);
        Assert.IsInstanceOfType(await controller.GetEnrollmentById(13), typeof(ForbidResult));
    }

    [TestMethod]
    public async Task EnrollmentNotFound()
    {
        var services = new IntegrationServices();
        services.CreateStudentAndSetAsCurrent();
        var provider = services.BuildServiceProvider();
        var controller = new Api.Controllers.Student.EnrollmentController(provider);
        Assert.IsInstanceOfType(await controller.GetEnrollmentById(13), typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task EnrollmentNotOfStudent()
    {
        var services = new IntegrationServices();
        services.CreateStudentAndSetAsCurrent();
        var otherStudent = services.CreateStudent(false, "other");
        var ( revision, _ ) = services.CreateCourse("course");
        var enrollment = services.AddEnrollment(otherStudent, revision, null);
        var provider = services.BuildServiceProvider();
        var controller = new Api.Controllers.Student.EnrollmentController(provider);
        Assert.IsInstanceOfType(await controller.GetEnrollmentById(enrollment.Id), typeof(ForbidResult));
    }

    [TestMethod]
    public async Task Ok()
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var ( revision, _ ) = services.CreateCourse("course");
        var enrollment = services.AddEnrollment(student, revision, null);
        var provider = services.BuildServiceProvider();
        var controller = new Api.Controllers.Student.EnrollmentController(provider);
        Assert.IsInstanceOfType(await controller.GetEnrollmentById(enrollment.Id), typeof(JsonResult));
    }
    
    [TestMethod]
    public async Task WithExistingSubmission()
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var ( revision, lesson ) = services.CreateCourse("course");
        var enrollment = services.AddEnrollment(student, revision, null);
        var submission = CreateSubmissionPostModel(lesson, services, false );
        
        var provider = services.BuildServiceProvider();
        var submitController = new Api.Controllers.Student.SubmitLessonController(provider);
        var submitResult = await submitController.SubmitLesson(submission) as JsonResult;
        
        var controller = new Api.Controllers.Student.EnrollmentController(provider);
        var result = await controller.GetEnrollmentById(enrollment.Id);
        
        Assert.IsInstanceOfType(submitResult, typeof(JsonResult));
        Assert.AreEqual(submitResult.StatusCode, 201);
        Assert.IsInstanceOfType(result, typeof(JsonResult));
    }
}