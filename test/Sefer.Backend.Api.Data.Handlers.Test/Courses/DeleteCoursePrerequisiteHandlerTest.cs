namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class DeleteCoursePrerequisiteHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var context = GetDataContext();
        await context.AddRangeAsync(course1, course2);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle_EntityNotFound()
    {
        var entity = new CoursePrerequisite { Id = 2 };
        await Handle(entity, false);
    }

    [TestMethod]
    public async Task Handle_NonExisting()
    {
        var entity = new CoursePrerequisite { Id = 1, CourseId = 1, RequiredCourseId = 2 };
        var context = GetDataContext();
        await context.AddAsync(entity);
        await context.SaveChangesAsync();

        entity.CourseId = 19;
        await Handle(entity, true);
    }

    [TestMethod]
    public async Task Handle()
    {
        var entity = new CoursePrerequisite { Id = 1, CourseId = 1, RequiredCourseId = 2 };
        var context = GetDataContext();
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
        await Handle(entity, true);
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var request = new DeleteCoursePrerequisiteRequest(null);
        var handler = new DeleteCoursePrerequisiteHandler(GetServiceProvider(new Exception()).Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(result);
    }

    private async Task Handle(CoursePrerequisite entity, bool deleted)
    {
        var request = new DeleteCoursePrerequisiteRequest(entity);
        var handler = new DeleteCoursePrerequisiteHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(deleted, result);
    }
}