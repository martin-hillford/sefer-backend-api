namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class UpdateCoursePrerequisiteHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        var course2 = new Course { Name = "course.2", Description = "course.2", Permalink = "course2" };
        var course3 = new Course { Name = "course.3", Description = "course.3", Permalink = "course3" };
        await InsertAsync(course1, course2, course3);
    }
    [TestMethod]
    public async Task Handle_SameId()
    {
        var course = await GetCourse("course.1");
        var provider = GetServiceProvider();
        await Handle(provider, course, course, false);
    }

    [TestMethod]
    public async Task Handle_CourseNull()
    {
        var course = await GetCourse("course.1");
        var required = await GetCourse("course.2");
        var provider = GetServiceProvider();
        await Handle(provider, course, required, false);
    }

    [TestMethod]
    public async Task Handle_RequireCourseNull()
    {
        var course = await GetCourse("course.1");
        var required = await GetCourse("course.2");
        var provider = GetServiceProvider();
        provider.AddRequestResults<GetCourseByIdRequest, Course>(course, null);
        await Handle(provider, course, required, false);
    }

    [TestMethod]
    public async Task Handle_Loop()
    {
        var course1 = await GetCourse("course.1");
        var course2 = await GetCourse("course.2");
        var course3 = await GetCourse("course.3");
        await InsertAsync(new CoursePrerequisite { CourseId = course1.Id, RequiredCourseId = course2.Id });
        await InsertAsync(new CoursePrerequisite { CourseId = course2.Id, RequiredCourseId = course3.Id });

        var provider = GetServiceProvider();
        provider.AddRequestResults<GetCourseByIdRequest, Course>(course3, course1);
        await Handle(provider, course3, course1, false);
    }

    [TestMethod]
    public async Task Handle_ExistingRequirement()
    {
        var course = await GetCourse("course.1");
        var required = await GetCourse("course.2");
        await InsertAsync(new CoursePrerequisite { CourseId = course.Id, RequiredCourseId = required.Id });

        var provider = GetServiceProvider();
        provider.AddRequestResults<GetCourseByIdRequest, Course>(course, required);
        await Handle(provider, course, required, true);
    }

    [TestMethod]
    public async Task Handle()
    {
        var course = await GetCourse("course.1");
        var required = await GetCourse("course.2");
        var provider = GetServiceProvider();
        provider.AddRequestResults<GetCourseByIdRequest, Course>(course, required);
        await Handle(provider, course, required, true);
    }

    private async Task<Course> GetCourse(string name)
    {
        var context = GetDataContext();
        return await context.Courses.SingleAsync(c => c.Name == name);
    }

    private static async Task Handle(MockedServiceProvider provider, Course course, Course required, bool expected)
    {
        var entity = new CoursePrerequisite { CourseId = course.Id, RequiredCourseId = required.Id };
        var request = new UpdateCoursePrerequisiteRequest(entity);
        var handler = new UpdateCoursePrerequisiteHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expected, result);
    }
}