namespace Sefer.Backend.Api.Test.Controllers.Admin;

public class AbstractAdminControllerTest : AbstractControllerTest
{
    // ReSharper disable once UnusedMember.Global
    protected static MockedServiceProvider GetServiceProvider()
    {
        var user = new User { Role = UserRoles.Admin, Id = 13 };
        return GetServiceProvider(user);
    }
}