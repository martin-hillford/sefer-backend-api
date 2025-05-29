namespace Sefer.Backend.Api.Notifications.WebSocket;

/// <summary>
/// This interface defines the websocket service can send.
/// Please note: regarding this interface as internal for Sefer.Backend.Api.Notifications.
/// use the INotificationService to ensure all notifications (including) are send
/// </summary>
public interface IWebSocketProvider
{
    public Task SendMessage(MessageView message, bool ignoreHasRead = false);

    public Task MarkMessageRead(int messageId, User user);

    public Task ChannelReceiversChanged(int channelId, List<User> previousReceivers, List<User> currentReceivers);
}