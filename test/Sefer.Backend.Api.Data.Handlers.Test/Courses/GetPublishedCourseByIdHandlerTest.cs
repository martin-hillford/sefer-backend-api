namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetPublishedCourseByIdHandlerTest : HandlerUnitTest
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
    public async Task Handle_CourseIdNull()
    {
        Assert.IsNull(await Handle(null));
    }
    
    [TestMethod]
    public async Task Handle_NoCourse()
    {
        Assert.IsNull(await Handle(_course?.Id + 1));
    }
    
    [TestMethod]
    public async Task Handle_NoPublishedCourse()
    {
        Assert.IsNull(await Handle(_course?.Id));
    }
    
    [TestMethod]
    public async Task Handle()
    {
        if(_course != null) await InsertAsync(new CourseRevision { CourseId = _course.Id, Stage = Stages.Published, Version = 1});
        var course = await Handle(_course?.Id);
        Assert.IsNotNull(course);
        Assert.IsNotNull(course.PublishedCourseRevision);
    }

    private async Task<Course> Handle(int? courseId)
    {
        var request = new GetPublishedCourseByIdRequest(courseId);
        var handler = new GetPublishedCourseByIdHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}