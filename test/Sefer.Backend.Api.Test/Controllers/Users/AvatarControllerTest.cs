using System.Net;
using System.Net.Http;
using Sefer.Backend.Api.Controllers.Users;
using Sefer.Backend.Api.Models.Users;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Services.HttpConnection;

namespace Sefer.Backend.Api.Test.Controllers.Users;

[TestClass]
public class AvatarControllerTest
{
    [TestMethod]
    public async Task UploadAvatar_NoUser()
    {
        var serviceProvider = MockedServiceProvider
            .Create()
            .SetupUserId(null)
            .AddService(new Mock<IHttpClientFactory>())
            .AddService(new Mock<IAvatarService>());

        var controller = new AvatarController(serviceProvider.Object);
        var body = new AvatarPostModel { Image = "image", Type = "image/jpg" };
        var result = await controller.UploadAvatar(body);

        Assert.IsNotNull(result as BadRequestResult);
    }

    [TestMethod]
    public async Task UploadAvatar_InvalidModel()
    {
        var serviceProvider = MockedServiceProvider
            .Create()
            .SetupUserId(10)
            .AddService(new Mock<IHttpClientFactory>())
            .AddService(new Mock<IAvatarService>());

        var controller = new AvatarController(serviceProvider.Object);
        var result = await controller.UploadAvatar(null);

        Assert.IsNotNull(result as BadRequestResult);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.BadRequest, 500)]
    [DataRow(HttpStatusCode.BadGateway, 500)]
    [DataRow(HttpStatusCode.OK, 204)]
    [DataRow(HttpStatusCode.NoContent, 204)]
    public async Task UploadAvatar(HttpStatusCode postResult, int expectedCode)
    {
        var serviceProvider = CreateProvider(postResult);

        var controller = new AvatarController(serviceProvider.Object);
        var body = new AvatarPostModel { Image = "image", Type = "image/jpg" };
        var result = await controller.UploadAvatar(body) as StatusCodeResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(expectedCode, result.StatusCode);
    }

    private static MockedServiceProvider CreateProvider(HttpStatusCode statusCode)
    {
        const string avatarUrl = "http://mock/upload";
        const int userId = 10;

        var httpClient = new Mock<IHttpClient>();
        var response = new HttpResponseMessage(statusCode);
        httpClient.Setup(c => c.PostAsJsonAsync(avatarUrl, It.IsAny<AvatarPostModel>(), null, CancellationToken.None)).ReturnsAsync(response);

        var avatarService = new Mock<IAvatarService>();
        avatarService.Setup(s => s.GetAvatarUploadUrl(userId)).Returns(avatarUrl);

        return MockedServiceProvider
            .Create()
            .SetupUserId(userId)
            .AddService(httpClient)
            .AddService(avatarService);
    }
}