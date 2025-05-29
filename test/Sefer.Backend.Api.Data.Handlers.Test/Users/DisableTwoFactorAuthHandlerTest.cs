namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class DisableTwoFactorAuthHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_NoUser()
    {
        var disabled = await Handle(1);
        Assert.IsFalse(disabled);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var user = new User { TwoFactorAuthEnabled = true, Name = "test", Email = "test@example.com" };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        Assert.IsTrue(await GetTwoFactorAuthEnabled(user.Id));

        var disabled = await Handle(user.Id);
        Assert.IsTrue(disabled);
        Assert.IsFalse(await GetTwoFactorAuthEnabled(user.Id));
    }

    private async Task<bool?> GetTwoFactorAuthEnabled(int userId)
    {
        var context = GetDataContext();
        var user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);
        return user?.TwoFactorAuthEnabled;
    }

    private async Task<bool> Handle(int userId)
    {
        var provider = GetServiceProvider().AddCaching();
        var request = new DisableTwoFactorAuthRequest(userId);
        var handler = new DisableTwoFactorAuthHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}