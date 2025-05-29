namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetCourseByPermalinkHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var course1 = new Course { Name = "course.1", Description = "course.1", Permalink = "course1", };
        var context = GetDataContext();
        await context.AddRangeAsync(course1);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var first = await context.Courses.FirstOrDefaultAsync();

        Assert.IsNotNull(first);

        var request = new GetCourseByPermalinkRequest(first.Permalink);
        var handler = new GetCourseByPermalinkHandler(GetServiceProvider().Object);
        var course = await handler.Handle(request, CancellationToken.None);

        Assert.IsNotNull(course);
        Assert.AreEqual(first.Id, course.Id);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    [DataRow("Permalink")]
    public async Task Handle_PermalinkNotFound(string permalink)
    {
        var request = new GetCourseByPermalinkRequest(permalink);
        var handler = new GetCourseByPermalinkHandler(GetServiceProvider().Object);
        var course = await handler.Handle(request, CancellationToken.None);

        Assert.IsNull(course);
    }
}