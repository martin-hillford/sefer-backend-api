namespace Sefer.Backend.Api.Data.Requests.Users;

public class UpdateUserRoleRequest(int userId, UserRoles role) : IRequest<bool>
{
    public readonly int UserId = userId;

    public readonly UserRoles Role = role;
}