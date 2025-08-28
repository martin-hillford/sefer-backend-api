// ReSharper disable InconsistentNaming
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Models.Settings;
using Sefer.Backend.Api.Data.Requests.Enrollments;
using Sefer.Backend.Api.Data.Requests.Settings;
using Student_EnrollmentView = Sefer.Backend.Api.Views.Student.EnrollmentView;

namespace Sefer.Backend.Api.Test.Controllers.Students;

public partial class DashboardControllerTest : AbstractControllerTest
{
    [TestMethod]
    public async Task GetActiveEnrollment_NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var result = await controller.GetActiveEnrollment();

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }
    
    [TestMethod]
    public async Task GetActiveEnrollment_Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var result = await controller.GetActiveEnrollment();

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task GetActiveEnrollment_MultipleActiveEnrollmentsEnabled()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, true);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var result = await controller.GetActiveEnrollment() as StatusCodeResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(418, result.StatusCode);
    }
    
    [TestMethod]
    public async Task GetActiveEnrollment_NoEnrollments()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, false);
        mocked.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([]);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var result = await controller.GetActiveEnrollment();

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
    
    [TestMethod]
    public async Task GetActiveEnrollment_Ok()
    {
        var enrollment = CreateEnrollment(13);
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, false);
        mocked.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([ enrollment ]);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActiveEnrollment();
        var view = (response as JsonResult)?.Value as Student_EnrollmentView; 
        
        Assert.IsInstanceOfType(response, typeof(JsonResult));
        Assert.IsNotNull(view);
        Assert.AreEqual(14, view.NextLesson.Id); // enrollmentId + 1
    }
    
    [TestMethod]
    public async Task GetActiveEnrollment_Ok_WithMultipleEnrollments()
    {
        var enrollmentOne = CreateEnrollment(7);
        var enrollmentTwo = CreateEnrollment(13);
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, false);
        mocked.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([ enrollmentOne, enrollmentTwo ]);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActiveEnrollment();
        var view = (response as JsonResult)?.Value as Student_EnrollmentView; 
        
        Assert.IsInstanceOfType(response, typeof(JsonResult));
        Assert.IsNotNull(view);
        Assert.AreEqual(8, view.NextLesson.Id); // enrollmentId + 1
    }
    
    [TestMethod]
    public async Task GetActiveEnrollments_NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var result = await controller.GetActiveEnrollments();

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }
    
    [TestMethod]
    public async Task GetActiveEnrollments_Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var result = await controller.GetActiveEnrollments();

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }
    
    [TestMethod]
    public async Task GetActiveEnrollments_NoEnrollments()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(student, false);
        mocked.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([]);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActiveEnrollments();

        var view = (response as JsonResult)?.Value as List<Student_EnrollmentView>; 
        Assert.IsInstanceOfType(response, typeof(JsonResult));
        Assert.AreEqual(0, view?.Count);
    }
    
    [TestMethod]
    public async Task GetActiveEnrollments_Ok()
    {
        var enrollment = CreateEnrollment(13);
        var mentor = new User { Id = 1, Role = UserRoles.Student };
        var mocked = Create(mentor, false);
        mocked.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([ enrollment ]);
        var controller = new Api.Controllers.Student.DashboardController(mocked.Object);

        var response = await controller.GetActiveEnrollments();
        var view = (response as JsonResult)?.Value as List<Student_EnrollmentView>; 
        
        Assert.IsInstanceOfType(response, typeof(JsonResult));
        Assert.IsNotNull(view);
        Assert.AreEqual(14, view.FirstOrDefault()?.NextLesson.Id); // enrollmentId + 1
    }
    
    private static MockedServiceProvider Create(User user, bool allowMultipleActiveEnrollments)
    {
        var settings = new Settings { AllowMultipleActiveEnrollments = allowMultipleActiveEnrollments };
        var mocked = GetServiceProvider(user);
        mocked.AddRequestResult<GetSettingsRequest, Settings>(settings);
        return mocked;
    }

    internal static Enrollment CreateEnrollment(int enrollmentId)
    {
        var course = new Data.Models.Courses.Course();
        var revision = new Data.Models.Courses.CourseRevision { Course = course };
        var lessonOne = new Data.Models.Courses.Lessons.Lesson { Id = enrollmentId + 1, CourseRevision = revision};
        var lessonTwo = new Data.Models.Courses.Lessons.Lesson { Id = enrollmentId + 2, CourseRevision = revision };
        revision.Lessons = [lessonOne, lessonTwo];
        var enrollment = new Enrollment { Id = enrollmentId, CourseRevision = revision };
        return enrollment;
    }
}



