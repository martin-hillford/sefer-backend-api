namespace Sefer.Backend.Api.Data.Handlers.Test.CourseRevisions;

[TestClass]
public class GetLessonsByCourseRevisionHandlerTest : GetLessonsUnitTest
{
    [TestMethod]
    public async Task Handle_NoCourseRevision()
    {
        var lessons = await Handle(19);
        Assert.AreEqual(0, lessons);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var provider = GetServiceProvider();
        var courseRevision = await context.CourseRevisions.SingleAsync(s => s.Id == 1);
        provider.AddRequestResult<GetCourseRevisionByIdRequest, CourseRevision>(courseRevision);
        var lessons = await Handle(courseRevision.Id, provider);
        Assert.AreEqual(2, lessons);
    }

    private async Task<int> Handle(int courseRevisionId, MockedServiceProvider? provider = null)
    {
        provider ??= GetServiceProvider();
        var request = new GetLessonsByCourseRevisionRequest(courseRevisionId);
        var handler = new GetLessonsByCourseRevisionHandler(provider.Object);
        var lessons = await handler.Handle(request, CancellationToken.None);
        return lessons.Count;
    }
}