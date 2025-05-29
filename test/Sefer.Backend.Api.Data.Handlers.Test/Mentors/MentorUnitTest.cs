namespace Sefer.Backend.Api.Data.Handlers.Test.Mentors;

public abstract class MentorUnitTest : HandlerUnitTest
{
    [TestInitialize]
    public virtual async Task Initialize()
    {
        var student = new User { Role = UserRoles.Student, Name = "Student", Gender = Genders.Male, PrimaryRegion = "nl", PrimarySite = "test.tld", Email = "student@example.tld", YearOfBirth = 1987 };
        var mentor = new User { Role = UserRoles.Mentor, Name = "Mentor", Gender = Genders.Male, PrimaryRegion = "nl", PrimarySite = "test.tld", Email = "mentor@example.tld", YearOfBirth = 1987 };
        await InsertAsync(student, mentor);
    }
}