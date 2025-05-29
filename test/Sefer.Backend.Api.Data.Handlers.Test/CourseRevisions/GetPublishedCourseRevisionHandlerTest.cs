namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class GetPublishedCourseRevisionHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Initialize()
    {
        await InsertAsync(new Course {Name = "course.1", Description = "course.1", Permalink = "course1", ShowOnHomepage = true});
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 1, Stage = Stages.Closed });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 2, Stage = Stages.Test });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 3, Stage = Stages.Closed });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 4, Stage = Stages.Edit });
        await InsertAsync(new CourseRevision { CourseId = 1, Version = 5, Stage = Stages.Published });
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetPublishedCourseRevisionRequest(1);
        var handler = new GetPublishedCourseRevisionHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(5, result.Version);
    }
}