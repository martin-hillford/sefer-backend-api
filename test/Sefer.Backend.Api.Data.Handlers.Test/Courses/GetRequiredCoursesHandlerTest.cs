namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetRequiredCoursesHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course3" };
        var course3 = new Course { Name = "course.3", Description = "course.3", Permalink = "course4" };

        await InsertAsync(course1, course2, course3);
        await InsertAsync(new CoursePrerequisite { CourseId = course1.Id, RequiredCourseId = course2.Id });
        await InsertAsync(new CoursePrerequisite { CourseId = course1.Id, RequiredCourseId = course3.Id });
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var course = await context.Courses.FirstAsync(c => c.Name == "course.1");
        var result = await Handle(course.Id);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("course.2", result.First().Name);
        Assert.AreEqual("course.3", result.Last().Name);
    }

    [TestMethod]
    public async Task Handle_NoCourse()
    {
        var result = await Handle(19);
        Assert.AreEqual(0, result.Count);
    }

    private async Task<List<Course>> Handle(int courseId)
    {
        var request = new GetRequiredCoursesRequest(courseId);
        var handler = new GetRequiredCoursesHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}