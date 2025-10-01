// ReSharper disable InconsistentNaming
using Sefer.Backend.Api.Controllers.Student;
using Sefer.Backend.Api.Data.Models.Courses.Lessons;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Models.Settings;

namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public partial class SubmitLessonControllerTest : AbstractControllerTest
{
    [TestMethod]
    public async Task IsStudentAllowedToSubmit_NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new SubmissionController(mocked.Object);

        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit());
        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit(13));
    }
    
    [TestMethod]
    public async Task IsStudentAllowedToSubmit_Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new SubmissionController(mocked.Object);

        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit());
        Assert.IsInstanceOfType<ForbidResult>(await controller.IsStudentAllowedToSubmit(13));
        
    }

    [TestMethod]
    public async Task IsStudentAllowedToSubmit_AllowMultipleActiveEnrollmentsError()
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
    public async Task IsStudentAllowedToSubmit_Ok_NoSubmissions()
    {
        var services = new IntegrationServices();
        services.CreateStudentAndSetAsCurrent();
        var provider = services.BuildServiceProvider();
        
        var controller = new SubmissionController(provider);
        var result = await controller.IsStudentAllowedToSubmit();
        
        Assert.IsInstanceOfType<OkResult>(result);
    }
    
    [TestMethod]
    public async Task IsStudentAllowedToSubmit_Ok_NoSubmissions_SpecificEnrollment()
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
    public async Task IsStudentAllowedToSubmit_NotFound_SpecificEnrollment()
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
    public async Task IsStudentAllowedToSubmit_ExistingSubmission(bool final, Type expectedType)
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

    [TestMethod]
    public async Task IsStudentAllowedToSubmit_Integration_SingleEnrollment()
    {
        var services = IntegrationServices.Create();
        IsStudentAllowedToSubmit_IntegrationSetup(services);
        var provider = services.BuildServiceProvider();
        var controller = new SubmissionController(provider);
        
        var result = await controller.IsStudentAllowedToSubmit();
        Assert.IsInstanceOfType<OkResult>(result);
    }
    
    [TestMethod]
    public async Task IsStudentAllowedToSubmit_Integration_NotAllowed()
    {
        var services = new IntegrationServices();
        var (enrollment, lesson) = IsStudentAllowedToSubmit_IntegrationSetup(services);
        var provider = services.BuildServiceProvider();
        var controller = new SubmissionController(provider);
        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = true, LessonId = lesson.Id, CreationDate = DateTime.UtcNow, SubmissionDate = DateTime.UtcNow };
        services.Provider.GetContext().Insert(submission);
        
        var result = await controller.IsStudentAllowedToSubmit();
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    [DataRow(0, 0, true, DisplayName = "NoRestrictions")]
    [DataRow(null, null, true, DisplayName = "NoRestrictions_SetByNull")]
    [DataRow(0, 1, false, DisplayName = "Overloaded")]
    [DataRow(null, 1, false, DisplayName = "Overloaded_SetByNull")]
    [DataRow(1, 0, false, DisplayName = "RestrictedGeneral")]
    [DataRow(1, null, false, DisplayName = "RestrictedGeneral_SetByNull")]
    [DataRow(0, 2, true, DisplayName = "Overloaded_ButAllowed")]
    [DataRow(2, 0, true, DisplayName = "Allowed_By_General")]
    [DataRow(2, null, true, DisplayName = "AllowedByGeneral_SpecificIsNull")]
    public async Task IsStudentAllowedToSubmit_Integration_Settings(int? generalMax, int? specialMax, bool allowed )
    {
        // Create the services and the base data
        var services = new IntegrationServices();
        var (enrollment, lesson) = IsStudentAllowedToSubmit_IntegrationSetup(services);
        
        // Set the generalMax
        var context = services.Provider.GetContext();
        var settings = context.Settings.First();
        settings.MaxLessonSubmissionsPerDay = (byte?)generalMax;
        context.Settings.Update(settings);
        await context.SaveChangesAsync(TestContext.CancellationTokenSource.Token);
        
        // Set the specialMax
        var courseRevision =  context.CourseRevisions.Single(c => c.Id == lesson.CourseRevisionId);
        var course = context.Courses.Single(c => c.Id == courseRevision.CourseId);
        course.MaxLessonSubmissionsPerDay =  specialMax;
        context.Courses.Update(course);
        await context.SaveChangesAsync(TestContext.CancellationTokenSource.Token);
        
        // Create an existing submission
        var provider = services.BuildServiceProvider();
        var controller = new SubmissionController(provider);
        var submission = new LessonSubmission { EnrollmentId = enrollment.Id, IsFinal = true, LessonId = lesson.Id, CreationDate = DateTime.UtcNow, SubmissionDate = DateTime.UtcNow };
        services.Provider.GetContext().Insert(submission);
        
        // After all this 
        var result = await controller.IsStudentAllowedToSubmit();
        
        // Now check the result 
        if(allowed) Assert.IsInstanceOfType<OkResult>(result);
        else Assert.IsInstanceOfType<NoContentResult>(result);
    }
    
    
    private static (Enrollment, Lesson) IsStudentAllowedToSubmit_IntegrationSetup(IntegrationServices services)
    {
        var student = services.CreateStudentAndSetAsCurrent();
        var  (revision, lesson) = services.CreateCourse("course");
        var mentor = services.CreateMentor();
        var enrollment = services.AddEnrollment(student, revision, mentor);
        services.SetSettings(new Settings { AllowMultipleActiveEnrollments = false, MaxLessonSubmissionsPerDay = 1 });
        return (enrollment, lesson);
    }

    public TestContext TestContext { get; set; }
}