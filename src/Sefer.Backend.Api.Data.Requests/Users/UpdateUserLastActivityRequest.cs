namespace Sefer.Backend.Api.Data.Requests.Users;

public class UpdateUserLastActivityRequest(int userId) : IRequest<bool>
{
    public readonly int UserId = userId;
}