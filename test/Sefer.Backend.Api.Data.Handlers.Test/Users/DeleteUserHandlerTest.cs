namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class DeleteUserHandlerTest : DeleteEntityHandlerTest<DeleteUserRequest, DeleteUserHandler, User>
{
    protected override async Task<List<(User, bool)>> GetTestData()
    {
        var existing = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        var context = GetDataContext();
        await context.AddAsync(existing);
        await context.SaveChangesAsync();

        var missing = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        return
        [
            (existing, true),
            (missing, false)
        ];
    }
}