namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class SwitchCourseRevisionToEditHandlerTest : HandlerUnitTest
{
    [TestMethod]
    [DataRow(Stages.Closed, false)]
    [DataRow(Stages.Published, false)]
    [DataRow(Stages.Test, true)]
    [DataRow(Stages.Edit, false)]
    public async Task Handle(Stages stage, bool shouldSwitch)
    {
        await InsertAsync(new Course {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true});
        await InsertAsync(new CourseRevision {CourseId = 1, Stage = stage, Version = 1});

        var request = new SwitchCourseRevisionToEditRequest(1);
        var handler = new SwitchCourseRevisionToEditHandler(ServiceProvider);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(shouldSwitch, result);
    }
}