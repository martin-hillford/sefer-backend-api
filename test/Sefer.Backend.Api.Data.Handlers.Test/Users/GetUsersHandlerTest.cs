namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class GetUsersHandlerTest : HandlerUnitTest
{
    [TestInitialize]
    public async Task Init()
    {
        var context = GetDataContext();
        await context.AddAsync(new User { Role = UserRoles.Admin, Name = "a", Gender = Genders.Male, Email = "a@e.tld", YearOfBirth = 1987 });
        await context.AddAsync(new User { Role = UserRoles.Admin, Name = "c", Gender = Genders.Male, Email = "b@e.tld", YearOfBirth = 1987 });
        await context.AddAsync(new User { Role = UserRoles.Mentor, Name = "b", Gender = Genders.Male, Email = "c@e.tld", YearOfBirth = 1987 });
        await context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task Handle()
    {
        var request = new GetUsersRequest();
        var handler = new GetUsersHandler(GetServiceProvider().Object);
        var result = await handler.Handle(request, CancellationToken.None);
        Assert.AreEqual(3, result.Count);

        Assert.AreEqual("a", result.First().Name);
        Assert.AreEqual("c", result.Last().Name);
    }
}

