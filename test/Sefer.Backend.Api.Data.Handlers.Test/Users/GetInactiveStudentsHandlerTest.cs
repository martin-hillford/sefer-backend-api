namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class GetInactiveStudentsHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle()
    {
        var request = new GetInactiveStudentsRequest(DateTime.Now.AddDays(-6));
        var handler = new GetInactiveStudentsHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(0, result.Count);
    }
}