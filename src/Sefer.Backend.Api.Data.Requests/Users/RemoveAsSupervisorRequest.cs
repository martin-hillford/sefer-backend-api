namespace Sefer.Backend.Api.Data.Requests.Users;

public class RemoveAsSupervisorRequest(int userId) : IRequest<bool>
{
    public readonly int UserId = userId;
}