// ReSharper disable InconsistentNaming

using Microsoft.Extensions.DependencyInjection;
using Sefer.Backend.Api.Controllers.Student;
using Sefer.Backend.Api.Data.Models.Settings;

namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public class SubmitLessonController_IsStudentAllowedToSubmit_Test : AbstractControllerTest
{
    [TestMethod]
    public async Task NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new SubmissionController(mocked.Object);

        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit());
        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit(13));
    }
    
    [TestMethod]
    public async Task Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new SubmissionController(mocked.Object);

        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit());
        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit(13));
        
    }

    [TestMethod]
    public async Task AllowMultipleActiveEnrollmentsError()
    {
        var services = new IntegrationServices();
        services.CreateStudentAndSetAsCurrent();
        services.SetSettings(new Settings { AllowMultipleActiveEnrollments = true });
        var provider = services.BuildServiceProvider();
        
        var controller = new SubmissionController(provider);
        var result = await controller.IsStudentAllowedToSubmit();
        
        Assert.IsInstanceOfType<StatusCodeResult>(result);
        Assert.AreEqual(418, (result as StatusCodeResult)?.StatusCode);
    }
    
    [TestMethod]
    public async Task Ok_NoSubmissions()
    {
        var services = new IntegrationServices();
        services.CreateStudentAndSetAsCurrent();
        var provider = services.BuildServiceProvider();
        
        var controller = new SubmissionController(provider);
        var result = await controller.IsStudentAllowedToSubmit();
        
        Assert.IsInstanceOfType<OkResult>(result);
    }
    
    [TestMethod]
    public async Task Ok_NoSubmissions_SpecificEnrollment()
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var ( revision, _ ) = services.CreateCourse("course");
        var enrollment = services.AddEnrollment(student, revision, null);
        var provider = services.BuildServiceProvider();
        
        var controller = new SubmissionController(provider);
        var result = await controller.IsStudentAllowedToSubmit(enrollment.Id);
        
        Assert.IsInstanceOfType<OkResult>(result);
    }
    
    [TestMethod]
    public async Task NotFound_SpecificEnrollment()
    {
        var services = new IntegrationServices();
        services.CreateStudentAndSetAsCurrent();
        var provider = services.BuildServiceProvider();
        
        var controller = new SubmissionController(provider);
        var result = await controller.IsStudentAllowedToSubmit(13);
        
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }
    
    [TestMethod]
    [DataRow(false, typeof(OkResult)), DataRow(true, typeof(NoContentResult))]
    public async Task ExistingSubmission(bool final, Type expectedType)
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var ( revision, lesson ) = services.CreateCourse("course");
        services.CreateLesson(revision, 2); // A second lesson is needed else the course will be finished
        var enrollment = services.AddEnrollment(student, revision, null);
        var provider = services.BuildServiceProvider();
        var submission = CreateSubmissionPostModel(lesson, services, final );
        
        var submitController = new SubmitLessonController(provider);
        var submitResult = await submitController.SubmitLesson(submission);
        
        var controller = new SubmissionController(provider);
        var result = await controller.IsStudentAllowedToSubmit(enrollment.Id);

        Assert.IsInstanceOfType(result, expectedType);
        Assert.IsInstanceOfType<JsonResult>(submitResult);
    }
}