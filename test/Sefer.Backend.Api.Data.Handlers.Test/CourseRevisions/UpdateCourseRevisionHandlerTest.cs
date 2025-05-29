namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class UpdateCourseRevisionHandlerTest
    : UpdateEntityHandlerTest<UpdateCourseRevisionRequest, UpdateCourseRevisionHandler, CourseRevision>
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1 });
    }

    protected override async Task<List<(CourseRevision entity, bool valid)>> GetTestData()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true });
        var existing = new CourseRevision { CourseId = 1, Version = 1 };
        var context = GetDataContext();
        await context.AddAsync(existing);
        await context.SaveChangesAsync();

        var missing = new CourseRevision();
        return
        [
            (existing, true),
            (missing, false)
        ];
    }
}