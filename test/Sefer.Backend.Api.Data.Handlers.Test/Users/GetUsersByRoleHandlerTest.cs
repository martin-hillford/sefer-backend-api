namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class GetUsersByRoleHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        await context.AddAsync(new User { Role = UserRoles.Admin, Name = "a", Gender = Genders.Male, Email = "a@e.tld", YearOfBirth = 1987 });
        await context.AddAsync(new User { Role = UserRoles.Admin, Name = "b", Gender = Genders.Male, Email = "b@e.tld", YearOfBirth = 1987 });
        await context.AddAsync(new User { Role = UserRoles.Mentor, Name = "b", Gender = Genders.Male, Email = "c@e.tld", YearOfBirth = 1987 });
        await context.SaveChangesAsync();
    }

    [TestMethod]
    [DataRow(UserRoles.Admin, 2)]
    [DataRow(UserRoles.Mentor, 1)]
    [DataRow(UserRoles.Student, 0)]
    public async Task Handle(UserRoles role, int expectedCount)
    {
        var request = new GetUsersByRoleRequest(role);
        var handler = new GetUsersByRoleHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(expectedCount, result.Count);
    }
}

