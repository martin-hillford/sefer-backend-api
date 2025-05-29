namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCourseStudentsCountHandlerTest : HandlerUnitTest
{
    private Course? _course1;
    
    private Course? _course2;
    
    [TestInitialize]
    public async Task Init()
    {
        _course1 = new Course {Name = "course.1", Description = "course.1", Permalink = "course1"};
        _course2 = new Course {Name = "course.2", Description = "course.2", Permalink = "course2"};
        await InsertAsync(_course1, _course2);
        
        var revision1 = new CourseRevision {CourseId = _course1.Id, Stage = Stages.Edit, Version = 1};
        var revision2 = new CourseRevision {CourseId = _course2.Id, Stage = Stages.Edit, Version = 1};
        await InsertAsync(revision1, revision2);
        
        var user1 = new User { Role = UserRoles.Admin, Name = "a", Gender = Genders.Male, Email = "a@e.tld", YearOfBirth = 1987};
        var user2 = new User { Role = UserRoles.Admin, Name = "b", Gender = Genders.Male, Email = "b@b.tld", YearOfBirth = 1986};
        await InsertAsync(user1, user2);
        
        var enrollment1 = new Enrollment {StudentId = user1.Id, CourseRevisionId = revision1.Id };
        var enrollment2 = new Enrollment {StudentId = user1.Id, CourseRevisionId = revision2.Id };
        var enrollment3 = new Enrollment {StudentId = user2.Id, CourseRevisionId = revision2.Id };
        await InsertAsync(enrollment1, enrollment2, enrollment3);
    }
    
    [TestMethod]
    public async Task Handle_CourseIdNull()
        => await Handle(0, null);
    
    [TestMethod]
    public async Task Handle_NoEnrollments()
        => await Handle(0, 19);

    [TestMethod]
    public async Task Handle()
    {
        Assert.IsNotNull(_course1);
        Assert.IsNotNull(_course2);
        
        await Handle(1, _course1.Id);
        await Handle(2, _course2.Id);
    }

    private async Task Handle(int expected, int? courseId)
    {
        var request = new GetCourseStudentsCountRequest(courseId);
        var handler = new GetCourseStudentsCountHandler(GetServiceProvider().Object);
        var count = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expected, count);
    }
}