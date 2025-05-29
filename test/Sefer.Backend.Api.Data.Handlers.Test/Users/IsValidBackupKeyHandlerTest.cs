namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class IsValidBackupKeyHandlerTest : HandlerUnitTest
{
    private User? _user;

    [TestInitialize]
    public async Task Init()
    {
        _user = new User { Name = "Admin", Id = 1, Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        var context = GetDataContext();
        await context.Users.AddAsync(_user);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle_InCorrectKey()
    {
        Assert.IsNotNull(_user);
        var context = GetDataContext();
        await context.AddAsync(new UserBackupKey { UserId = _user.Id, BackupKey = "key" });
        await context.SaveChangesAsync();
        Assert.IsFalse(await Handle(_user.Id, "key"));
    }

    [TestMethod]
    public async Task Handle_NoUser()
    {
        Assert.IsFalse(await Handle(13, "key"));
    }

    [TestMethod]
    public async Task Handle_CorrectKey()
    {
        Assert.IsNotNull(_user);
        var context = GetDataContext();
        await context.AddAsync(new UserBackupKey { UserId = _user.Id, BackupKey = Hashing.Sha512("key") });
        await context.SaveChangesAsync();
        Assert.IsTrue(await Handle(_user.Id, "key"));
    }

    private async Task<bool> Handle(int userId, string key)
    {
        var request = new IsValidBackupKeyRequest(userId, key);
        var handler = new IsValidBackupKeyHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}