namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class CloseCourseRevisionHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        await InsertAsync(new Course { Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true });
    }

    [TestMethod]
    public async Task Handle_CourseRevisionNull()
    {
        var request = new CloseCourseRevisionRequest(19);
        var handler = new CloseCourseRevisionHandler(GetServiceProvider().Object);
        var closed = await handler.Handle(request, CancellationToken.None);
        Assert.IsFalse(closed);
    }

    [TestMethod]
    [DataRow(Stages.Closed, false)]
    [DataRow(Stages.Edit, false)]
    [DataRow(Stages.Published, true)]
    [DataRow(Stages.Test, false)]
    public async Task Handle(Stages stage, bool expected)
    {
        var context = GetDataContext();
        var course = await context.Courses.FirstAsync();
        var revision = new CourseRevision { Version = 1, CourseId = course.Id, Stage = stage };
        await InsertAsync(revision);

        var request = new CloseCourseRevisionRequest(revision.Id);
        var handler = new CloseCourseRevisionHandler(GetServiceProvider().Object);
        var closed = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(expected, closed);
        if (!closed) return;

        var reloaded = await context.CourseRevisions.FirstAsync();
        Assert.AreEqual(Stages.Closed, reloaded.Stage);
    }
}