namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetUsersByRoleRequest(UserRoles role) : IRequest<List<User>>
{
    public readonly UserRoles Role = role;
}