namespace Sefer.Backend.Api.Notifications.Mail;

/// <summary>
/// This interface defines the e-mail the service can send.
/// Please note: regarding this interface as internal for Sefer.Backend.Api.Notifications.
/// Use the INotificationService to ensure all notifications are sent
/// </summary>

public interface IEmailDigestService
{
    public Task SendDirectNotificationsDigestAsync();

    public Task SendDailyNotificationsDigestAsync();

    public Task SendWeeklyNotificationsDigestAsync();

    public Task SendNotificationsDigestAsync(NotificationPreference notificationPreference, int delay = 0);

    // ReSharper disable once UnusedMember.Global
    public Task SendNotificationsDigestAsync(User user);
}