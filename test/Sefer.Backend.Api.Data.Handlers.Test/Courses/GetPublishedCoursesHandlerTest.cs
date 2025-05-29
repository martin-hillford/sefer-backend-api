namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetPublishedCoursesHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course1 = new Course {Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course {Name = "course.2", Description = "course.2", Permalink = "course3" };
        var course3 = new Course {Name = "course.3", Description = "course.3", Permalink = "course4" };
        
        await InsertAsync(course1, course2, course3);
        await InsertAsync(new CourseRevision {CourseId = course1.Id, Stage = Stages.Edit, Version = 1});
        await InsertAsync(new CourseRevision {CourseId = course2.Id, Stage = Stages.Published, Version = 1});
        await InsertAsync(new CourseRevision {CourseId = course3.Id, Stage = Stages.Published, Version = 1});
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetPublishedCoursesRequest();
        var handler = new GetPublishedCoursesHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("course.2", result.First().Name);
        Assert.AreEqual("course.3", result.Last().Name);
    }
}