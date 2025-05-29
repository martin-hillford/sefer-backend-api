namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetUsersForNotificationRequest(NotificationPreference mailPreference) : IRequest<List<User>>
{
    public readonly NotificationPreference MailPreference = mailPreference;
}