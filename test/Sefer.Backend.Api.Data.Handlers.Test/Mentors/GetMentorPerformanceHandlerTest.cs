namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetMentorPerformanceHandlerTest : MentorUnitTest
{
    [TestMethod]
    [ExpectedException(typeof(SqliteException))]
    public async Task Handle()
    {
        // This method just tests of the view is used
        var request = new GetMentorPerformanceRequest();
        var handler = new GetMentorPerformanceHandler(GetServiceProvider().Object);
        await handler.Handle(request, CancellationToken.None);
    }
}