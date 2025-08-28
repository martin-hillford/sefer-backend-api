namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

[TestClass]
public class GetMentorPerformanceHandlerTest : MentorUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        // This method just tests of the view is used
        var request = new GetMentorPerformanceRequest();
        var handler = new GetMentorPerformanceHandler(GetServiceProvider().Object);
        await Assert.ThrowsExactlyAsync<SqliteException>(async () => await handler.Handle(request, CancellationToken.None));
    }
}