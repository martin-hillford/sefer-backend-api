namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class GetPublicAdminHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_NoSettings()
    {
        var admin = await GetPublicAdmin();
        Assert.IsNull(admin);
    }

    [TestMethod]
    public async Task Handle()
    {
        var context = GetDataContext();
        var a = new User { Role = UserRoles.Admin, Name = "a", Gender = Genders.Male, Email = "a@e.tld", YearOfBirth = 1987 };
        var b = new User { Role = UserRoles.Admin, Name = "b", Gender = Genders.Male, Email = "b@e.tld", YearOfBirth = 1987 };
        var c = new User { Role = UserRoles.Mentor, Name = "b", Gender = Genders.Male, Email = "c@e.tld", YearOfBirth = 1987 };
        await context.AddAsync(a);
        await context.AddAsync(b);
        await context.AddAsync(c);
        await context.SaveChangesAsync();
        await context.AddAsync(new AdminSettings { IsPublicAdmin = true, AdminId = a.Id });
        await context.AddAsync(new AdminSettings { IsPublicAdmin = false, AdminId = b.Id });
        await context.AddAsync(new AdminSettings { IsPublicAdmin = true, AdminId = c.Id });
        await context.SaveChangesAsync();

        var admin = await GetPublicAdmin();
        Assert.IsNotNull(admin);
        Assert.AreEqual("a", admin.Name);
    }

    [TestMethod]
    public async Task Handle_NoAdmin()
    {
        var context = GetDataContext();
        await context.AddAsync(new User { Name = "a", Gender = Genders.Male, Email = "a@e.tld", YearOfBirth = 1987 });
        await context.SaveChangesAsync();
        await context.AddAsync(new AdminSettings { IsPublicAdmin = false, AdminId = 1 });
        await context.SaveChangesAsync();

        var admin = await GetPublicAdmin();
        Assert.IsNull(admin);
    }

    private async Task<User> GetPublicAdmin()
    {
        var request = new GetPublicAdminRequest();
        var handler = new GetPublicAdminHandler(GetServiceProvider().Object);
        return await handler.Handle(request, CancellationToken.None);
    }
}