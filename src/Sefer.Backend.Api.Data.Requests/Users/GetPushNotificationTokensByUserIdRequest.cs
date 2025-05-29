namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetPushNotificationTokensByUserIdRequest(int userId) : IRequest<List<string>>
{
    public readonly int UserId = userId;
}