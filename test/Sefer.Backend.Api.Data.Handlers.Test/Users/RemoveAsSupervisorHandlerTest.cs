namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class RemoveAsSupervisorHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_NoUser()
    {
        var provider = new MockedServiceProvider();
        var result = await Handle(19, provider);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_NoSupervisor()
    {
        var provider = new MockedServiceProvider();
        provider.AddCaching();
        provider.AddRequestResult<GetUserByIdRequest, User>(new User());
        var result = await Handle(23, provider);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle()
    {
        var provider = new MockedServiceProvider();
        provider.AddCaching();
        provider.AddRequestResult<GetUserByIdRequest, User>(new User { Role = UserRoles.Supervisor });
        provider.AddRequestResult<RemoveAsMentorRequest, bool>(true);
        var result = await Handle(37, provider);
        Assert.IsTrue(result);
    }

    private static async Task<bool> Handle(int userId, MockedServiceProvider provider)
    {
        var request = new RemoveAsSupervisorRequest(userId);
        var handler = new RemoveAsSupervisorHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}