namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class AddCourseHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_EntityNull() => await Handle(false, null);

    [TestMethod]
    public async Task Handle_EntityIdNotZero() => await Handle(false, new Course { Id = 1 });

    [TestMethod]
    public async Task Handle_InValid() => await Handle(false, new Course());

    [TestMethod]
    public async Task Handle()
    {
        var course = new Course { Name = "course.1", Description = "course.1", Permalink = "course1" };
        await Handle(true, course);

        var context = GetDataContext();

        var courseRevision = await context.CourseRevisions.FirstOrDefaultAsync();
        Assert.IsNotNull(courseRevision);

        var survey = await context.Surveys.FirstOrDefaultAsync();
        Assert.IsNotNull(survey);
    }

    private async Task Handle(bool added, Course? course, MockedServiceProvider? provider = null)
    {
        var request = new AddCourseRequest(course);
        var handler = new AddCourseHandler(provider?.Object ?? GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(added, result);
    }
}