namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class UpdateCourseHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course2" };

        await InsertAsync(course1, course2);
        await InsertAsync(new CourseRevision { CourseId = course1.Id, Stage = Stages.Edit, Version = 1 });
        await InsertAsync(new CourseRevision { CourseId = course2.Id, Stage = Stages.Published, Version = 1 });
    }

    [TestMethod]
    public async Task Handle_NotValid()
    {
        var course = new Course();
        await Handle(course, false);
    }

    [TestMethod]
    public async Task Handle_CourseNotFound()
    {
        var course = new Course { Name = "course.3", Description = "course.3", Permalink = "course3", Id = 3 };
        await Handle(course, false);
    }

    [TestMethod]
    public async Task Handle_EditableCourse()
    {
        var context = GetDataContext();
        var course = context.Courses.Single(c => c.Name == "course.1");
        await Handle(course, true);
    }

    [TestMethod]
    public async Task Handle_PublishedCourse()
    {
        var context = GetDataContext();
        var course = context.Courses.Single(c => c.Name == "course.2");
        await Handle(course, true);
    }

    private async Task Handle(Course course, bool updateExpected)
    {
        var context = GetDataContext();
        var provider = GetServiceProvider();

        var courses = await context.Courses.Include(c => c.CourseRevisions).ToListAsync();
        foreach (var existing in courses.Where(existing => course.Id == existing.Id))
        {
            provider.AddRequestResult<GetCourseByIdRequest, Course>(existing);
        }

        provider.AddRequestResult<UpdateCourseRequest, bool>(true);

        var request = new UpdateCourseRequest(course);
        var handler = new UpdateCourseHandler(provider.Object);
        var updated = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(updateExpected, updated);
    }
}