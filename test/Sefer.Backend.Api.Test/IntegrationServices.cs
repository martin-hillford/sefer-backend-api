using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sefer.Backend.Api.Data;
using Sefer.Backend.Api.Data.Handlers;
using Sefer.Backend.Api.Data.Models.Courses;
using Sefer.Backend.Api.Data.Models.Courses.Lessons;
using Sefer.Backend.Api.Data.Models.Courses.Surveys;
using Sefer.Backend.Api.Data.Models.Enrollments;
using Sefer.Backend.Api.Data.Models.Settings;

namespace Sefer.Backend.Api.Test;

// This service provider is used to be able to test more complex scenario's using an in-memory sql-lite database
// This changes the test behavior to more integration like tests.
public class IntegrationServices : ServiceCollection
{
    public readonly IDataContextProvider Provider;
    
    public IntegrationServices()
    {
        Provider = SetUpDatabase();
        this
            .AddMediation()
            .AddLogging()
            .AddSingleton(Provider)
            .AddMemoryCache();
    }

    public static IntegrationServices Create() => [];

    private static DataContextProvider SetUpDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;
        using var context = new DataContext(options);
        context.Database.EnsureCreated();

        return new DataContextProvider(options);
    }
}

public static class Extensions
{
    public static User CreateStudentAndSetAsCurrent(this IntegrationServices services)
        => CreateStudent(services, true);
    
    public static User CreateStudent(this IntegrationServices services, bool addAsCurrentUser = false, string name = "student")
    {
        var student = new User { Role = UserRoles.Student, Name = name, Email = $"student_{Guid.NewGuid()}@test.local"};
        return CreateUser(services, student, addAsCurrentUser); 
    }
    
    public static User CreateMentor(this IntegrationServices services, bool addAsCurrentUser = false)
    {
        var mentor = new User { Role = UserRoles.Mentor, Name  = "Mentor", Email = "mentor@test.local"};
        return CreateUser(services, mentor, addAsCurrentUser); 
    }

    private static User CreateUser(this IntegrationServices services, User user, bool addAsCurrentUser = false)
    {
        var context = services.Provider.GetContext();
        context.Users.Add(user);
        context.SaveChanges();
        if (addAsCurrentUser) services.AddCurrentUser(user);
        return user;  
    }
    
    private static void AddMockedService<TService>(this IntegrationServices services, Mock<TService> service) where TService : class
    {
        services.AddSingleton(service.Object);
    }
    
    private static void AddCurrentUser(this IntegrationServices services, User user)
    {
        var authService = new Mock<IUserAuthenticationService>();
        authService.Setup(a => a.UserId).Returns(user.Id);
        authService.Setup(a => a.IsAuthenticated).Returns(true);
        authService.Setup(a => a.UserRole).Returns(user.Role);
        services.AddMockedService(authService);
    }

    public static (CourseRevision, Lesson) CreateCourse(this IntegrationServices services, string name, bool allowSelfStudy = false)
    {
        var context = services.Provider.GetContext();
        var course = new Course { Name = name, Description = "Description", Permalink = "permalink"  };
        context.Insert(course);

        var courseRevision = new CourseRevision { CourseId = course.Id, Course = course, AllowSelfStudy = allowSelfStudy };
        context.Insert(courseRevision);

        var lesson = CreateLesson(services, courseRevision, 1);
        var survey = new Survey { CourseRevisionId = courseRevision.Id };
        context.Insert(survey);

        return (courseRevision, lesson);
    }

    public static Lesson CreateLesson(this IntegrationServices services, CourseRevision courseRevision, int sequenceId)
    {
        var context = services.Provider.GetContext();
        var lesson = new Lesson { Name = "Lesson", Number = "1", CourseRevisionId = courseRevision.Id, SequenceId = sequenceId };
        context.Insert(lesson);
        var boolQuestion = new BoolQuestion { Content = "Question", CorrectAnswerIsTrue = true, LessonId = lesson.Id };
        context.Insert(boolQuestion);
        var openQuestion = new OpenQuestion { Content = "Question", LessonId = lesson.Id };
        context.Insert(openQuestion);
        var multipleQuestion = new MultipleChoiceQuestion { Content = "Question", LessonId = lesson.Id };
        context.Insert(multipleQuestion);
        context.Insert(new MultipleChoiceQuestionChoice { QuestionId = multipleQuestion.Id, Answer = "A", IsCorrectAnswer = true});
        context.Insert(new MultipleChoiceQuestionChoice { QuestionId = multipleQuestion.Id, Answer = "B", IsCorrectAnswer = false});
        return lesson;
    }

    public static Enrollment AddEnrollment(this IntegrationServices services, User student, CourseRevision courseRevision, User mentor, bool imported = false)
    {
        var enrollment = new Enrollment
        {
            StudentId = student.Id, 
            CourseRevisionId = courseRevision.Id,
            MentorId = mentor?.Id,
            Imported = imported
        };
        services.Provider.GetContext().Insert(enrollment);
        return enrollment;
    }

    public static void Insert<T>(this DataContext context, T entity)
    {
        context.Add(entity);
        context.SaveChanges();
    }

    public static void SetSettings(this IntegrationServices services, Settings settings)
    {
        var context = services.Provider.GetContext();
        context.Settings.RemoveRange(context.Settings);
        context.SaveChanges();
        context.Add(settings);
        context.SaveChanges();
    }
    
    public static object GetValueOrNull(this object dyn, string name)
    {
        try {  return dyn.GetType().GetProperty(name)?.GetValue(dyn); }
        catch (Exception) { return null; }
    }
}