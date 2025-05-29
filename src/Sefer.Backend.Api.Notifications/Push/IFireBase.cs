namespace Sefer.Backend.Api.Notifications.Push;

/// <summary>
/// This interface defines a method to send push notifications
/// Please note: regarding this interface as internal for Sefer.Backend.Api.Notifications.
/// use the INotificationService to ensure all notifications (including) are send
/// </summary>
public interface IFireBase
{
    Task<List<Exception>> SendMessage(int userId, string title, string body);
}
