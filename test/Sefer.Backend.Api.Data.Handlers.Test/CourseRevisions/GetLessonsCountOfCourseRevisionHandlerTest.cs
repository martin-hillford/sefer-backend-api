namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class GetLessonsCountOfCourseRevisionHandlerTest : GetLessonsUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetLessonsCountOfCourseRevisionRequest(1);
        var handler = new GetLessonsCountOfCourseRevisionHandler(ServiceProvider);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(2, result);
    }
}