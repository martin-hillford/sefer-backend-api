namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetHomepageCoursesHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var course1 = new Course {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true};
        var course2 = new Course {Name = "course.2", Description = "course.2", Permalink = "course2"};
        var course3 = new Course {Name = "course.3", Description = "course.3", Permalink = "course3", ShowOnHomepage = true};
        await InsertAsync(course1, course2, course3);
        
        await InsertAsync(new CourseRevision {CourseId = course1.Id, Stage = Stages.Edit, Version = 1});
        await InsertAsync(new CourseRevision {CourseId = course2.Id, Stage = Stages.Edit, Version = 2});
        await InsertAsync(new CourseRevision {CourseId = course3.Id, Stage = Stages.Edit, Version = 2});
        
        await InsertAsync(new CourseRevision {CourseId = course2.Id, Stage = Stages.Published, Version = 1});
        await InsertAsync(new CourseRevision {CourseId = course3.Id, Stage = Stages.Published, Version = 1});
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetHomepageCoursesRequest();
        var handler = new GetHomepageCoursesHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("course.3", result.First().Name);
    }
}