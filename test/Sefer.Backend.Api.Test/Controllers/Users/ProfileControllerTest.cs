using Sefer.Backend.Api.Controllers.Users;
using Sefer.Backend.Api.Data.Requests.Settings;
using Sefer.Backend.Api.Models.Users;
using Sefer.Backend.Api.Services.Notifications;

namespace Sefer.Backend.Api.Test.Controllers.Users;

[TestClass]
public class ProfileControllerTest
{
    [TestMethod]
    public async Task UpdatePassword_PostNull()
    {
        var serviceProvider = MockedServiceProvider.Create();
        var controller = new ProfileController(serviceProvider.Object);
        var result = await controller.UpdatePassword(null);
        Assert.IsNotNull(result as BadRequestResult);
    }

    [TestMethod]
    public async Task UpdatePassword_PasswordsNotTheSame()
    {
        var serviceProvider = MockedServiceProvider.Create();
        var controller = new ProfileController(serviceProvider.Object);
        var post = new UpdatePasswordPostModel { ConfirmNewPassword = "new", Password = "password" };
        var result = await controller.UpdatePassword(post);
        Assert.IsNotNull(result as BadRequestResult);
    }

    [TestMethod]
    public async Task UpdatePassword_NoUser()
    {
        var serviceProvider = MockedServiceProvider.Create();
        var controller = new ProfileController(serviceProvider.Object);
        var post = new UpdatePasswordPostModel { ConfirmNewPassword = "new", Password = "new" };
        var result = await controller.UpdatePassword(post);
        Assert.IsNotNull(result as UnauthorizedResult);
    }

    [TestMethod]
    public async Task UpdatePassword_InValidPassword()
    {
        var user = new User { Id = 10, PrimaryRegion = "nl", PrimarySite = "test.tld" };
        var passwordService = new Mock<IPasswordService>();
        passwordService.Setup(s => s.IsValidPassword(user, "old")).Returns(false);

        var site = new Mock<ISite>();
        site.SetupGet(s => s.SiteUrl).Returns("https://test.tld");
        site.SetupGet(s => s.Hostname).Returns("test.tld");

        var serviceProvider = MockedServiceProvider.Create()
            .SetupUserId(10)
            .AddRequestResult<GetUserByIdRequest, User>(user)
            .AddService(passwordService)
            .AddRequestResult<GetSiteByNameRequest, ISite>(site.Object);

        var controller = new ProfileController(serviceProvider.Object);
        var post = new UpdatePasswordPostModel { Language = "nl", ConfirmNewPassword = "new", Password = "new", OldPassword = "old" };
        var result = await controller.UpdatePassword(post);

        Assert.IsNotNull(result as ForbidResult);
    }

    [TestMethod]
    public async Task UpdatePassword()
    {
        var user = new User { Id = 10, PrimaryRegion = "nl", PrimarySite = "test.tld" };
        var passwordService = new Mock<IPasswordService>();
        var site = new Mock<ISite>();
        site.SetupGet(s => s.SiteUrl).Returns("https://test.tld");
        site.SetupGet(s => s.Hostname).Returns("test.tld");
        passwordService.Setup(s => s.IsValidPassword(user, "old")).Returns(true);

        var notificationService = new Mock<INotificationService>();
        var serviceProvider = MockedServiceProvider.Create()
            .SetupUserId(10)
            .AddRequestResult<GetUserByIdRequest, User>(user)
            .AddRequestResult<UpdateUserRequest, bool>(true)
            .AddRequestResult<GetSiteByNameRequest, ISite>(site.Object)
            .AddService(passwordService)
            .AddService(notificationService);

        var controller = new ProfileController(serviceProvider.Object);
        var post = new UpdatePasswordPostModel { ConfirmNewPassword = "new", Language = "fr", Password = "new", OldPassword = "old" };
        var result = await controller.UpdatePassword(post);

        Assert.IsNotNull(result as AcceptedResult);
        notificationService.Verify(m => m.SendPasswordResetCompletedNotificationAsync(It.IsAny<User>(), "fr"));
    }
}