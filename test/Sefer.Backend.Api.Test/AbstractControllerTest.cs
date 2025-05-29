namespace Sefer.Backend.Api.Test;

public abstract class AbstractControllerTest
{
    protected static MockedServiceProvider GetServiceProvider(User currentUser)
    {
        var authService = new Mock<IUserAuthenticationService>();
        authService.Setup(a => a.UserId).Returns(currentUser.Id);
        var provider = new MockedServiceProvider();
        provider.AddService(authService);
        provider.AddRequestResult<GetUserByIdRequest, User>(currentUser);
        return provider;
    }
}