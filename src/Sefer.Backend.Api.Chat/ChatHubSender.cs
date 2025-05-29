namespace Sefer.Backend.Api.Chat;

/// <summary>
/// A simple helper for the chat hub to wrap static methods
/// </summary>
public static class ChatHubSender
{
    /// <summary>
    /// This simple method is capable of sending the message view to the receiver
    /// </summary>
    /// <param name="client">The context for sending messages</param>
    /// <param name="message">The message to send</param>
    public static async Task<bool> SendMessage(IClientProxy client, MessageView message)
    {
        try
        {
            if (client == null) return false;
            await client.SendAsync("onMessageReceive", message);
            return true;
        }
        catch (Exception) { return false; }
    }

    public static async Task<bool> MarkMessageRead(IClientProxy client, int channelId, int messageId, User user)
    {
        try
        {
            var view = new { MessageId = messageId, ChannelId = channelId, UserId = user.Id, UserName = user.Name };
            await client.SendAsync("onMessageRead", view);
            return true;
        }
        catch (Exception) { return false; }
    }
}
