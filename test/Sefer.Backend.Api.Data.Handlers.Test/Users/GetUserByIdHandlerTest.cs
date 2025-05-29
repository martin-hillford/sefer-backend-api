namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class GetUserByIdHandlerTest : GetEntityByIdHandlerTest<GetUserByIdRequest, GetUserByIdHandler, User>
{
    protected override Task<User> GetEntity()
    {
        var user = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        return Task.FromResult(user);
    }
}
