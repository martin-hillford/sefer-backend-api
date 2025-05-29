using Sefer.Backend.Api.Views.Public.Users;

namespace Sefer.Backend.Api.Test.Controllers.Public;

[TestClass]
public class AccountControllerTest
{
    [TestMethod]
    public async Task HasUserUserTwoAuthEnabled_NoUser()
    {
        var serviceProvider = MockedServiceProvider.Create().SetupUserId(null);
        var controller = new AccountController(serviceProvider.Object);
        var response = await controller.HasUserUserTwoAuthEnabled();
        Assert.IsNotNull(response as UnauthorizedResult);
    }

    [TestMethod]
    [DataRow(true), DataRow(false)]
    public async Task HasUserUserTwoAuthEnabled(bool enabled)
    {
        var user = new User { TwoFactorAuthEnabled = enabled };
        var serviceProvider = MockedServiceProvider.Create()
            .SetupUserId(10)
            .AddRequestResult<GetUserByIdRequest, User>(user);
        var controller = new AccountController(serviceProvider.Object);
        var response = await controller.HasUserUserTwoAuthEnabled() as JsonResult;

        Assert.IsNotNull(response?.Value);
        Assert.AreEqual(enabled, response != null && ((UserUserTwoAuthEnabledView)response.Value).Enabled);
    }

    [TestMethod]
    public async Task EmergencyLogin_InvalidLogin()
    {
        // Arrange
        var provider = GetServiceProvider(SignOnResult.Success);

        var controller = new AccountController(provider.Object);
        var post = new EmergencyLogonModel();

        // Act
        var result = await controller.EmergencyLogin(post);

        // Assert
        Assert.IsTrue(result is UnauthorizedResult);

    }

    [TestMethod]
    public async Task EmergencyLogin_NoUser()
    {
        // Arrange
        var provider = GetServiceProvider(SignOnResult.Success);
        provider.AddRequestResult<GetUserByEmailRequest, User>(null);

        var controller = new AccountController(provider.Object);
        var post = new EmergencyLogonModel();

        // Act
        var result = await controller.EmergencyLogin(post);

        // Assert
        Assert.IsTrue(result is UnauthorizedResult);
    }

    private static MockedServiceProvider GetServiceProvider(SignOnResult result)
    {
        var authService = new Mock<IUserAuthenticationService>();
        authService.Setup(m => m.SignOn(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(result);

        var securityOptions = new SecurityOptions();
        var options = Options.Create(securityOptions);

        var provider = new MockedServiceProvider();
        provider.Setup(p => p.GetService(typeof(IUserAuthenticationService))).Returns(authService.Object);
        provider.Setup(p => p.GetService(typeof(IOptions<SecurityOptions>))).Returns(options);

        return provider;
    }
}