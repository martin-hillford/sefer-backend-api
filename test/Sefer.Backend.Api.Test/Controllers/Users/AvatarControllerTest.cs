using System.IO;
using System.Text.Json;
using Sefer.Backend.Api.Controllers.Users;
using Sefer.Backend.Api.Models.Users;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Shared;

namespace Sefer.Backend.Api.Test.Controllers.Users;

[TestClass]
public class AvatarControllerTest
{
    private string _tempDirectory;

    private const string HashKey = "1IsCbQq6qVHDFJOdbelkWZkd7dvHKANqt6zNhnwadDmQDqfYOXDk1kA4F3EdNO40";

    private const string Base64Img = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAQAAADZc7J/AAABuUlEQVR4AcXV22oTQRjA8X0QX6Pv9m1tWbTUNkGaVqJlFGmFttQgLhiVeKiuSK0XPUDr4aYX0W3SpOvs9gX07zguiszFmA0if/Zu5jc7G8gXBMGtCzc3VF8xWqtf424ykfzYrrSiWivfkonAnE5MyvmIpcSYnd3AvHyF7ZawFwkUivOKKdO/ADTHtKgxRUSTF3wiHwU45Sk1hJBJk5iWeIP+W2DAA0KEOrfZZIMmlxDzbDHED6B5Zk9e5j1DcjQpr1lAiHhO4QeOqRPa7b8XFRzQQLhO3w+0EK7ywVm4zyzT7JD7gBohq2QO0OMOQpvMB0xxkXvuXcl4iLDJqQ+ImOQuuQvwCKHFwAc0EZbpOUCfNcQgX3zAFiEzbDuXOOKK/YiFD/jMEsIiB38s7bKO0CD1/4w5b5mxi/c4ITP1OGSdaYQ6H71vYNK85DJinhXaPGaNWaRM8c4LmM4MoYiQsogG84jtBkcUHsA2YJcO94npsMsJh1wriTm2yb2ATTPkDG1PLNhjsSRq7PgBt5z9X19joQJgiYQ5hIgnlQCT5hXzdMgc4P/+K489WMYebeMP16TieFf9n+P9O5w8Ic4gfhV1AAAAAElFTkSuQmCC";
    
    [TestInitialize]
    public void Setup()
    {
        _tempDirectory = FileUtils.GetTemporaryDirectory();
    }

    [TestCleanup]
    public void Cleanup()
    {
        Directory.Delete(_tempDirectory, true);
    }

    [TestMethod]
    public async Task GetAvatar_Unknown()
    {
        var (controller, avatarService) = CreateController();
        var hash = avatarService.GetAvatarId(93);
        var result = await controller.GetAvatarCached(hash, "TE", "black", "white");
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<ContentResult>(result);
        
        var contentResult = result as ContentResult;
        Assert.IsNotNull(contentResult);
        
        Assert.AreEqual(200, contentResult.StatusCode);
        Assert.StartsWith("<svg xmlns=", contentResult.Content);
        Assert.AreEqual("image/svg+xml", contentResult.ContentType);
    }

    [TestMethod]
    public async Task GetAvatar_Existing()
    {
        await UploadAvatar(); // reuse the upload test. this is similar what happens in user land
        var (controller, avatarService) = CreateController();
        var hash = avatarService.GetAvatarId(11);
        var result = await controller.GetAvatarCached(hash, "TE", "black", "white");
        
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<FileContentResult>(result);
        
        var contentResult = result as FileContentResult;
        Assert.IsNotNull(contentResult);

        Assert.AreEqual("image/test", contentResult.ContentType);
        
        var file = Convert.ToBase64String(contentResult.FileContents);
        Assert.AreEqual(Base64Img, file);
    }

    [TestMethod]
    public async Task UploadAvatar_ModelInvalid()
    {
        var (controller, _) = CreateController();
        var result = await controller.UploadAvatar(null);
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }

    [TestMethod]
    public async Task UploadAvatar_UserNotSet()
    {
        var body = new AvatarPostModel { Image = "image", Type = "image/test" };
        var (controller, _) = CreateController();
        var result = await controller.UploadAvatar(body);
        Assert.IsInstanceOfType<BadRequestResult>(result);
    }
    
    [TestMethod]
    public async Task UploadAvatar()
    {
        var user = new User { Id = 11, Email = "user@test.local" };
        var body = new AvatarPostModel { Image = Base64Img, Type = "image/test" };
        var (controller, avatarService) = CreateController(user);
        var hash = avatarService.GetAvatarId(user.Id);
        
        var result = await controller.UploadAvatar(body);
        
        Assert.IsInstanceOfType<OkResult>(result);
        
        var expectedFile = Path.Combine(_tempDirectory, $"{hash}.json");
        Assert.IsTrue(File.Exists(expectedFile));
        
        var content = await File.ReadAllTextAsync(expectedFile, TestContext.CancellationTokenSource.Token);
        var data = Response.FromBase64(Base64Img, "image/test");
        var dataJson = JsonSerializer.Serialize(data);
        Assert.AreEqual(dataJson, content);
    }
    
    private (AvatarController, AvatarService) CreateController(User currentUser = null)
    {
        var avatarOptions = new AvatarOptions
        {
            HashKey = HashKey,
            Service = "https://tests.local",
            Store = _tempDirectory,
            UseBlob = false
        };
        var provider = new MockedServiceProvider();
        var options = Options.Create(avatarOptions);
        var avatarService = AvatarService.Create(options);
        provider.AddService(options);
        provider.AddService<IAvatarService>(avatarService);
        if (currentUser != null) provider.SetupUser(currentUser);
        return (new AvatarController(provider.Object), avatarService);
    }

    public TestContext TestContext { get; set; }
}