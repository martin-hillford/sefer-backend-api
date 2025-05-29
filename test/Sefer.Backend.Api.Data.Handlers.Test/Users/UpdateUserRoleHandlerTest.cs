namespace Sefer.Backend.Api.Data.Handlers.Test.Users;

[TestClass]
public class UpdateUserRoleHandlerTest : HandlerUnitTest
{
    [TestMethod]
    public async Task Handle_NoUser()
    {
        var provider = GetServiceProvider();
        var result = await Handle(UserRoles.Admin, provider);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle_SameRole()
    {
        var provider = GetServiceProvider().AddCaching();
        var user = GetUser(UserRoles.Admin);
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        var result = await Handle(UserRoles.Admin, provider);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task Handle_UserToStudent()
    {
        var provider = GetServiceProvider().AddCaching();
        var user = GetUser(UserRoles.User);
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        provider.AddRequestResult<UpdateSingleUserPropertyRequest, bool>(true);
        var result = await Handle(UserRoles.Student, provider);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task Handle_UpdateFailed()
    {
        var provider = GetServiceProvider().AddCaching();
        var user = GetUser(UserRoles.Student);
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        provider.AddRequestResult<UpdateSingleUserPropertyRequest, bool>(false);
        var result = await Handle(UserRoles.User, provider);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task Handle()
    {
        var roles = TypeExtensions.GetEnumList<UserRoles>();
        foreach (var current in roles)
        {
            var toSet = roles.Where(role => role != current);
            foreach (var role in toSet)
            {
                await Handle(current, role);
            }
        }
    }
    private async Task Handle(UserRoles current, UserRoles role)
    {
        var provider = GetServiceProvider().AddCaching();
        var user = GetUser(current);
        provider.AddRequestResult<GetUserByIdRequest, User>(user);
        provider.AddRequestResult<UpdateSingleUserPropertyRequest, bool>(true);

        provider.AddRequestResult<RemoveStudentRoleRequest, bool>(true);
        provider.AddRequestResult<RemoveAsMentorRequest, bool>(true);
        provider.AddRequestResult<RemoveAsSupervisorRequest, bool>(true);
        provider.AddRequestResult<EnsureMentorSettingsRequest, MentorSettings>(new MentorSettings());

        var result = await Handle(role, provider);
        Assert.IsTrue(result);
    }


    private static async Task<bool> Handle(UserRoles role, MockedServiceProvider provider)
    {
        var request = new UpdateUserRoleRequest(0, role);
        var handler = new UpdateUserRoleHandler(provider.Object);
        return await handler.Handle(request, CancellationToken.None);
    }

    private static User GetUser(UserRoles role)
        => new() { Name = "Admin", Gender = Genders.Male, Email = "test@example.tld", YearOfBirth = 1987, Role = role };
}