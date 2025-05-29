namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetPublishedCourseByPermalinkHandlerTest : HandlerUnitTest
{
    private Course? _course;
    
    [TestInitialize]
    public async Task Init()
    {
        _course = new Course {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true};
        await InsertAsync(_course);
        await InsertAsync(new CourseRevision {CourseId = _course.Id, Stage = Stages.Edit, Version = 1});
    }

    [TestMethod]
    [DataRow(null), DataRow(""), DataRow(" ")]
    public async Task Handle_PermalinkEmpty(string permalink)
    {
        Assert.IsNull(await Handle(permalink));
    }
    
    [TestMethod]
    public async Task Handle_NoCourse()
    {
        Assert.IsNull(await Handle("no-permalink"));
    }
    
    [TestMethod]
    public async Task Handle_NoPublishedCourse()
    {
        Assert.IsNull(await Handle(_course?.Permalink));
    }
    
    [TestMethod]
    public async Task Handle()
    {
        if(_course != null) await InsertAsync(new CourseRevision {CourseId = _course.Id, Stage = Stages.Published, Version = 1});
        var course = await Handle(_course?.Permalink);
        Assert.IsNotNull(course);
        Assert.IsNotNull(course.PublishedCourseRevision);
    }

    private async Task<Course> Handle(string? permalink)
    {
        var request = new GetPublishedCourseByPermalinkRequest(permalink);
        var handler = new GetPublishedCourseByPermalinkHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}