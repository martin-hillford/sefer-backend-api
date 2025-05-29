namespace Sefer.Backend.Api.Data.Handlers.Test.Courses;

[TestClass]
public class GetStudentsCountPerCourseHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetStudentsCountPerCourseRequest();
        var handler = new GetStudentsCountPerCourseHandler(GetServiceProvider().Object);
        await Assert.ThrowsExactlyAsync<SqliteException>(async () => await handler.Handle(request, CancellationToken.None));
    }
}