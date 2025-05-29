namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCoursesWithRevisionHandlerTest  : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var course1 = new Course {Name = "course.1", Description = "course.1", Permalink = "course1"};
        var course2 = new Course {Name = "course.2", Description = "course.2", Permalink = "course2"};
        await InsertAsync(course1, course2);
        
        var revision1 = new CourseRevision {CourseId = course1.Id, Stage = Stages.Edit, Version = 1};
        var revision2 = new CourseRevision {CourseId = course2.Id, Stage = Stages.Edit, Version = 1};
        await InsertAsync(revision1, revision2);
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetCoursesWithRevisionRequest();
        var handler = new GetCoursesWithRevisionHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("course.1", result.First().Name);
        Assert.IsNotNull(result.First().CourseRevisions);
        Assert.IsNotNull(result.Last().CourseRevisions);
        Assert.AreEqual(1, result.First().CourseRevisions.Count);
        Assert.AreEqual(1, result.Last().CourseRevisions.Count);
    }
}