namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class UpdateBackupKeysHandlerTest : HandlerUnitTest
{
    private User? _user;

    [TestInitialize]
    public async Task Init()
    {
        _user = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        var context = GetDataContext();
        await context.Users.AddAsync(_user);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle_Exception()
    {
        var provider = GetServiceProvider(new Exception());
        var handler = new UpdateBackupKeysHandler(provider.Object);
        var result = await handler.Handle(null, CancellationToken.None);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_NoExistingKeys()
    {
        var provider = GetServiceProvider().AddCaching();
        var request = new UpdateBackupKeysRequest(_user, ["key"]);
        var handler = new UpdateBackupKeysHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.IsTrue(result);

        var context = GetDataContext();
        var count = await context.UserBackupKeys.CountAsync();
        Assert.AreEqual(1, count);
    }

    [TestMethod]
    public async Task Handle()
    {
        Assert.IsNotNull(_user);

        var context = GetDataContext();
        await context.AddAsync(new UserBackupKey { UserId = _user.Id, BackupKey = "key" });
        await context.SaveChangesAsync();
        var provider = GetServiceProvider().AddCaching();

        var request = new UpdateBackupKeysRequest(_user, ["key"]);
        var handler = new UpdateBackupKeysHandler(provider.Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.IsTrue(result);

        var keys = await context.UserBackupKeys.ToListAsync();
        Assert.AreEqual(1, keys.Count);
        Assert.AreNotEqual("key", keys.First().BackupKey);
    }
}