namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class GetUserByEmailHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var user = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        var context = GetDataContext();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle()
    {
        Assert.IsNotNull(await GetUserByEmail("test@example.tld"));
        Assert.IsNotNull(await GetUserByEmail("tesT@example.Tld"));
    }

    [TestMethod]
    public async Task Handle_NotFound()
    {
        Assert.IsNull(await GetUserByEmail("tst@example.tld"));
        Assert.IsNull(await GetUserByEmail("test@example.ld"));
    }

    [TestMethod]
    [DataRow(null), DataRow(""), DataRow(" ")]
    public async Task Handle_NullEmpty(string value)
    {
        var user = await GetUserByEmail(value);
        Assert.IsNull(user);
    }

    private async Task<User> GetUserByEmail(string? email)
    {
        var handler = new GetUserByEmailHandler(GetServiceProvider().Object);
        var request = new GetUserByEmailRequest(email);
        return await handler.Handle(request, CancellationToken.None);
    }
}