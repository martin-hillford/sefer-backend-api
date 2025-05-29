// ReSharper disable NullableWarningSuppressionIsUsed
namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class DeleteCourseHandlerTest
    : DeleteEntityHandlerTest<DeleteCourseRequest, DeleteCourseHandler, Course>
{
    private List<(Course Entity, bool IsValid)>? _data;

    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();

        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course2" };
        var course3 = new Course { Name = "course.3", Description = "course.3", Permalink = "course3" };
        await context.AddRangeAsync(course1, course2);
        await context.SaveChangesAsync();

        var revision1 = new CourseRevision { CourseId = course2.Id, Stage = Stages.Edit, Version = 1 };
        await context.AddRangeAsync(revision1);
        await context.SaveChangesAsync();

        _data =
        [
            (course1, false),
            (course2, true),
            (course3, false),
            (course3, false)
        ];
    }

    protected override Task<List<(Course Entity, bool IsValid)>> GetTestData()
    {
        Assert.IsNotNull(_data);
        return Task.FromResult(_data);
    }

    protected override MockedServiceProvider GetServiceProvider()
    {
        var provider = base.GetServiceProvider();
        Assert.IsNotNull(_data);

        var data = _data.Select(t => t.Entity).ToList();
        data.Add(null!);
        provider.AddRequestResults<GetCourseByIdRequest, Course>(data);

        return provider;
    }
}