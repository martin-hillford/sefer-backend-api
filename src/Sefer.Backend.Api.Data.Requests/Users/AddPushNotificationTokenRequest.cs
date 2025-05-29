namespace Sefer.Backend.Api.Data.Requests.Users;

public class AddPushNotificationTokenRequest(PushNotificationToken data) : IRequest<bool>
{
    public int UserId => data.UserId;

    public string Token => data.Token;
}