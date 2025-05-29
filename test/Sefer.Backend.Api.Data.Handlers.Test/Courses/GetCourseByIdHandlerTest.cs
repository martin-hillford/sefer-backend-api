namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCourseByIdHandlerTest : GetEntityByIdHandlerTest<GetCourseByIdRequest, GetCourseByIdHandler, Course>
{
    protected override Task<Course> GetEntity()
    {
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1", };
        return Task.FromResult(course);
    }

    [TestMethod]
    public async Task Handle_WithRevision()
    {
        var context = GetDataContext();

        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1", };
        await context.AddRangeAsync(course);
        await context.SaveChangesAsync();

        var revision = new CourseRevision { CourseId = course.Id, Stage = Stages.Edit, Version = 1 };
        await context.AddAsync(revision);
        await context.SaveChangesAsync();

        var request = new GetCourseByIdRequest(course.Id, true);
        var handler = new GetCourseByIdHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(result?.CourseRevisions);
    }
}