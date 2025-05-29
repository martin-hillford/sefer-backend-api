namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class AddUserHandlerTest : AddEntityHandlerTest<AddUserRequest, AddUserHandler, User>
{
    protected override List<(User, bool)> GetTestData() =>
    [
        (new User { Name = "test" }, false),
        (new User { Name = "test1", }, false),
        (new User { Name = "test1", Email = "@" }, false),
        (new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 }, true),
        (new User { Name = "Admin", Gender = Genders.Male, Email = "test.example.tld", YearOfBirth = 1987 }, false),
        (new User { Name = "Admin", Gender = Genders.Female, Email = "test1@example.tld", YearOfBirth = 1987 }, true),
        (new User { Name = "Admin", Gender = Genders.Female, Email = "test1@example.tld", YearOfBirth = 1987 }, false)
    ];
}