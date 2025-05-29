namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class UpdateSingleUserPropertyHandlerTest :
    UpdateSingleEntityPropertyHandlerTest<UpdateSingleUserPropertyRequest, UpdateSingleUserPropertyHandler, User>
{
    protected override async Task<List<(User entity, string property, object newValue, bool updated)>> GetTestData()
    {
        var context = GetDataContext();
        var added = new User { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987 };
        await context.Users.AddAsync(added);
        await context.SaveChangesAsync();
        Assert.AreEqual(1, await context.Users.CountAsync());

        return
        [
            (new User { Name = "mens", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987, Id = added.Id },
                "Name", "mens", true),
            (new User { Name = "mens", Gender = Genders.Female, Email = "test@example.tld", YearOfBirth = 1987, Id = added.Id },
                "Gender", Genders.Female, true),
            (new User { Name = "mens", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987, Id = 0 },
                "Gender", Genders.Female, false),
            (new User { Name = "mens", Gender = Genders.Male, Email = string.Empty, YearOfBirth = 1987, Id = added.Id },
                "Gender", Genders.Female, false)
        ];
    }
}