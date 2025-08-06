// ReSharper disable InconsistentNaming
using Microsoft.Extensions.DependencyInjection;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Models.Settings;
using Sefer.Backend.Api.Data.Requests.CourseRevisions;
using Sefer.Backend.Api.Data.Requests.Enrollments;
using Sefer.Backend.Api.Data.Requests.Lessons;
using Sefer.Backend.Api.Data.Requests.Settings;
using Sefer.Backend.Api.Data.Requests.Submissions;
using Sefer.Backend.Api.Models.Student.Profile;
using BoolQuestion = Sefer.Backend.Api.Data.Models.Courses.Lessons.BoolQuestion;
using Lesson = Sefer.Backend.Api.Data.Models.Courses.Lessons.Lesson;
using SubmitLessonController = Sefer.Backend.Api.Controllers.Student.SubmitLessonController;

namespace Sefer.Backend.Api.Test.Controllers.Students;

[TestClass]
public class SubmitLessonController_SubmitLesson_Test : AbstractControllerTest
{
    [TestMethod]
    public async Task NoUser()
    {
        var mocked = GetServiceProvider();
        var controller = new SubmitLessonController(mocked.Object);

        var result = await controller.SubmitLesson(new SubmissionPostModel());

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }
    
    [TestMethod]
    public async Task Mentor()
    {
        var mentor = new User { Id = 1, Role = UserRoles.Mentor};
        var mocked = GetServiceProvider(mentor);
        var controller = new SubmitLessonController(mocked.Object);

        var result = await controller.SubmitLesson(new SubmissionPostModel());

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task EnrollmentIdMissing()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var provider = Create(student, true);
        var controller = new SubmitLessonController(provider.Object);
        
        var result = await controller.SubmitLesson(new SubmissionPostModel());

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
    
    [TestMethod]
    public async Task NoActiveEnrollments()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var provider = Create(student, false);
        provider.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([]);
        var controller = new SubmitLessonController(provider.Object);
        
        var result = await controller.SubmitLesson(new SubmissionPostModel());

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task OnPaper()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var provider = Create(student, false);
        var enrollment = DashboardController_GetEnrollment_Test.CreateEnrollment(13);
        enrollment.OnPaper = true;
        provider.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([ enrollment ]);
        var controller = new SubmitLessonController(provider.Object);
        
        var result = await controller.SubmitLesson(new SubmissionPostModel());

        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    }
    
    [TestMethod]
    public async Task NoLesson()
    {
        var student = new User { Id = 1, Role = UserRoles.Student };
        var provider = Create(student, false);
        var enrollment = DashboardController_GetEnrollment_Test.CreateEnrollment(13);
        provider.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([ enrollment ]);
        var controller = new SubmitLessonController(provider.Object);
        
        var result = await controller.SubmitLesson(new SubmissionPostModel());

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task SelfStudyMissingCourseRevision()
    {
        var data = new SubmissionCreation { SaveSubmission = false };
        var (provider, submission, enrollment) = CreateValidSubmission(data);
        enrollment.MentorId = null;

        provider.AddRequestResult<GetSubmissionAnswersRequest, List<QuestionAnswer>>([]);
        var controller = new SubmitLessonController(provider.Object);
        
        var result = await controller.SubmitLesson(submission);
        
        Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
        Assert.AreEqual((result as StatusCodeResult)?.StatusCode, 500);
    }
    
    [TestMethod]
    [DataRow(false), DataRow(true)]
    public async Task ValidFinalSubmission(bool selfStudy)
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var mentor = services.CreateMentor();
        var (courseRevision, lesson) = services.CreateCourse("course 1", selfStudy);
        services.AddEnrollment(student,courseRevision, selfStudy ? null : mentor);
        var controller = new SubmitLessonController(services.BuildServiceProvider());
        var submission = CreateSubmissionPostModel(lesson, services, true );
        
        var result = await controller.SubmitLesson(submission);
        
        Assert.IsInstanceOfType(result, typeof(JsonResult));
        Assert.AreEqual((result as JsonResult)?.StatusCode, 202);
    }
    
    [TestMethod]
    public async Task ExistingNonFinal()
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var mentor = services.CreateMentor();
        var (courseRevision, lesson) = services.CreateCourse("course 1");
        services.AddEnrollment(student,courseRevision, mentor);
        var controller = new SubmitLessonController(services.BuildServiceProvider());
        
        var nonFinal = CreateSubmissionPostModel(lesson, services, false );
        var resultNonFinal = await controller.SubmitLesson(nonFinal);
        
        Assert.IsInstanceOfType(resultNonFinal, typeof(JsonResult));
        Assert.AreEqual((resultNonFinal as JsonResult)?.StatusCode, 201); // 201: saved but not final
        
        var final = CreateSubmissionPostModel(lesson, services, true );
        var resultFinal = await controller.SubmitLesson(final);
        
        Assert.IsInstanceOfType(resultFinal, typeof(JsonResult));
        Assert.AreEqual((resultFinal as JsonResult)?.StatusCode, 202); // 202: saved but not final
    }
    
    [TestMethod]
    public async Task NonFinal()
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var mentor = services.CreateMentor();
        var (courseRevision, _ ) = services.CreateCourse("course 1");
        var enrollment = services.AddEnrollment(student,courseRevision, mentor);
        
        var controller = new SubmitLessonController(services.BuildServiceProvider());
        var submission = new SubmissionPostModel { EnrollmentId = enrollment.Id, Final = false };
        
        var result = await controller.SubmitLesson(submission);
        
        Assert.IsInstanceOfType(result, typeof(JsonResult));
        Assert.AreEqual((result as JsonResult)?.StatusCode, 201);
    }
    
    [TestMethod]
    [DataRow(false, false), DataRow(true, false), DataRow(false, true), DataRow(true, true)]
    public async Task Imported(bool useMentor, bool final)
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var mentor = useMentor ? services.CreateMentor() : null;
        
        var (courseRevision, lesson) = services.CreateCourse("course 1");
        services.AddEnrollment(student,courseRevision, mentor, true);
        
        var controller = new SubmitLessonController(services.BuildServiceProvider());
        var submission = CreateSubmissionPostModel(lesson, services, final );
        
        var result = await controller.SubmitLesson(submission);
        
        Assert.IsInstanceOfType(result, typeof(BadRequestResult));
    }
    
    [TestMethod]
    [DataRow((byte)1), DataRow((byte)10)]
    public async Task MultipleLesson(byte maxLessonSubmissionsPerDay)
    {
        var services = new IntegrationServices();
        var student = services.CreateStudentAndSetAsCurrent();
        var mentor = services.CreateMentor();
        var (courseRevision, lessonA) = services.CreateCourse("course 1");
        services.AddEnrollment(student,courseRevision, mentor);
        var controller = new SubmitLessonController(services.BuildServiceProvider());
        var lessonB = services.CreateLesson(courseRevision, 2);

        var settings = new Settings { MaxLessonSubmissionsPerDay = maxLessonSubmissionsPerDay };
        services.SetSettings(settings);
        
        var submissionA = CreateSubmissionPostModel(lessonA, services, true );
        var resultA = await controller.SubmitLesson(submissionA);
        
        var submissionB = CreateSubmissionPostModel(lessonB, services, true );
        var resultB = await controller.SubmitLesson(submissionB);
        
        Assert.IsInstanceOfType(resultA, typeof(JsonResult));
        Assert.AreEqual((resultA as JsonResult)?.StatusCode, 201);

        if (maxLessonSubmissionsPerDay > 1)
        {
            Assert.IsInstanceOfType(resultB, typeof(JsonResult));
            Assert.AreEqual((resultB as JsonResult)?.StatusCode, 202);    
        }
        else
        {
            Assert.IsInstanceOfType(resultB, typeof(BadRequestObjectResult));
        }
    }

    
    
    #region Helper
    
    private static MockedServiceProvider Create(User user, bool allowMultipleActiveEnrollments)
    {
        var settings = new Settings { AllowMultipleActiveEnrollments = allowMultipleActiveEnrollments };
        var mocked = GetServiceProvider(user);
        mocked.AddRequestResult<GetSettingsRequest, Settings>(settings);
        return mocked;
    }
    
    private class SubmissionCreation
    {
        public bool AddCourseRevision { get; init; } = true;
        
        public bool SaveSubmission { get; init; } = true;
    }
    
    private static (MockedServiceProvider, SubmissionPostModel, Enrollment) CreateValidSubmission(SubmissionCreation data)
    {
        var course = new Data.Models.Courses.Course { Id = 29 };
        var courseRevision = new Data.Models.Courses.CourseRevision { Id = 13, Course = course, CourseId = 29 };
        var boolQuestion = new BoolQuestion { Id = 3, CorrectAnswerIsTrue = true, Heading = "Q1" };
        var lesson = new Lesson { Content = [boolQuestion], CourseRevision = courseRevision, CourseRevisionId = 13 };
        var answer = new QuestionAnswerPostModel { Answer = "Wrong", QuestionId = 3, QuestionType = ContentBlockTypes.QuestionBoolean};
        var submission = new SubmissionPostModel { Final = true, Answers = [answer] };
        var enrollment = new Enrollment { Id = 23 };

        var student = new User { Id = 1, Role = UserRoles.Student };
        var provider = Create(student, false);
        
        provider.AddRequestResult<GetActiveEnrollmentsOfStudentRequest, List<Enrollment>>([ enrollment ]);
        provider.AddRequestResult<GetCurrentLessonRequest,(Lesson, LessonSubmission, Enrollment)>((lesson, null, enrollment));
        if(data.AddCourseRevision) provider.AddRequestResult<GetCourseRevisionByIdRequest, Data.Models.Courses.CourseRevision>(courseRevision);
        provider.AddRequestResult<SaveSubmissionRequest, bool>(data.SaveSubmission);
        provider.AddRequestResult<GetBoolQuestionByIdRequest, BoolQuestion>(boolQuestion);
        
        return (provider, submission, enrollment);
    }
    

    #endregion
}