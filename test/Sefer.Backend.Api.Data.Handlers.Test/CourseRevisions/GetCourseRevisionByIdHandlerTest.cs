namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class GetCourseRevisionByIdHandlerTest
    : GetEntityByIdHandlerTest<GetCourseRevisionByIdRequest, GetCourseRevisionByIdHandler, CourseRevision>
{
    [TestInitialize]
    public async Task Initialize()
    {
        await InsertAsync(new Course
            {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true});
    }

    protected override Task<CourseRevision> GetEntity() => Task.FromResult(new CourseRevision {Version = 1, CourseId = 1});
}