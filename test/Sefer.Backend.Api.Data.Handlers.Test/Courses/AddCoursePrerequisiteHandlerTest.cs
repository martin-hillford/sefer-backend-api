namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class AddCoursePrerequisiteHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course1" };
        var course3 = new Course { Name = "course.3", Description = "course.3", Permalink = "course1" };
        await InsertAsync(course1, course2, course3);
    }

    [TestMethod]
    public async Task Handle_SameCourse()
        => await Handle(19, 19, false);

    [TestMethod]
    public async Task Handle_CourseNotFound()
        => await Handle(19, 21, false);

    [TestMethod]
    public async Task Handle_RequiredCourseNotFound()
        => await Handle(1, 21, false);

    [TestMethod]
    public async Task Handle_CreateLoop_Direct()
    {
        await AddCoursePrerequisite(1, 2);
        await Handle(2, 1, false);
    }

    [TestMethod]
    public async Task Handle_CreateLoop_Recursive()
    {
        await AddCoursePrerequisite(1, 2);
        await AddCoursePrerequisite(2, 3);
        await Handle(3, 1, false);
    }

    [TestMethod]
    public async Task Handle_Exists()
    {
        await AddCoursePrerequisite(1, 2);
        await Handle(1, 2, true);
    }

    [TestMethod]
    public async Task Handle()
        => await Handle(1, 2, true);

    private async Task Handle(int courseId, int requiredCourseId, bool added)
    {
        var entity = new CoursePrerequisite { CourseId = courseId, RequiredCourseId = requiredCourseId };
        var request = new AddCoursePrerequisiteRequest(entity);
        var handler = new AddCoursePrerequisiteHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(added, result);
    }

    private async Task AddCoursePrerequisite(int courseId, int requiredCourseId)
    {
        var entity = new CoursePrerequisite { CourseId = courseId, RequiredCourseId = requiredCourseId };
        var context = GetDataContext();
        await context.AddRangeAsync(entity);
        await context.SaveChangesAsync();
    }
}