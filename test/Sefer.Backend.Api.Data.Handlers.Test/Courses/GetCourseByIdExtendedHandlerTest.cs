namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCourseByIdExtendedHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var course = new Course {Name = "course.1", Description = "course.1", Permalink = "course1"};
        await InsertAsync(course);
        
        var revision = new CourseRevision {CourseId = course.Id, Stage = Stages.Edit, Version = 1};
        await InsertAsync(revision);
        
        var lessonA = new Lesson {Name = "A", Description = "A", Number = "A", CourseRevisionId = revision.Id, SequenceId = 2};
        var lessonB = new Lesson {Name = "B", Description = "B", Number = "B", CourseRevisionId = revision.Id, SequenceId = 1};
        await InsertAsync(lessonA, lessonB);
    }
    
    [TestMethod]
    public async Task Handle_NoCourse()
    {
        var course = await Handle(19);
        Assert.IsNull(course);
    }
    
    [TestMethod]
    public async Task Handle()
    {
        var course = await Handle(1);
        Assert.IsNotNull(course?.EditingCourseRevision);
        
        Assert.AreEqual("B", course.EditingCourseRevision.Lessons.First().Name);
        Assert.AreEqual("A", course.EditingCourseRevision.Lessons.Last().Name);
    }    

    private async Task<Course?> Handle(int courseId, MockedServiceProvider? provider = null)
    {
        var request = new GetCourseByIdExtendedRequest(courseId);
        var handler = new GetCourseByIdExtendedHandler(provider?.Object ?? GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}