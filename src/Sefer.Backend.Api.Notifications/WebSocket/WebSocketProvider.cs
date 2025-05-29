namespace Sefer.Backend.Api.Notifications.WebSocket;

/// <summary>
/// The notification providers deals with sending direct notifications to users.
/// It will try to use the chat hub to directly communicate a message.
///
/// Email notifications are handled by the notification service. So if a user is not only,
/// a few minutes later the notification service will pick any unread message and send the message
/// directly after that
/// </summary>
public class WebSocketProvider(IHubContext<ChatHub> chatHubContext, IMediator mediator) : IWebSocketProvider
{
    public async Task ChannelReceiversChanged(int channelId, List<User> previousReceivers, List<User> currentReceivers)
    {
        // Get all the users that are involved in the channel
        var users = previousReceivers
            .Select(user => user.Id).Union(currentReceivers.Select(user => user.Id)).Distinct();

        var clients = chatHubContext.Clients.Users(users.Select(user => user.ToString()).Distinct());
        var receivers = currentReceivers.Select(user => user.Id).Distinct();
        
        var data = new { channelId, receivers };
        await clients.SendAsync("onChannelReceiversChanged", data);
    }
    
    public async Task SendMessage(MessageView message, bool ignoreHasRead = false)
    {
        // First step: get the channel of the message
        var channelMessage = await mediator.Send(new GetChannelMessageRequest(message.UserId, message.Id));
        if (channelMessage == null) return;
        if (ignoreHasRead == false && channelMessage.ReadDate != null) return;

        // Now send a message to the clients
        var client = chatHubContext.Clients.User(message.UserId.ToString());
        if (client != null) await ChatHubSender.SendMessage(client, message);
    }

    public async Task MarkMessageRead(int messageId, User user)
    {
        try
        {
            var channelMessage = await mediator.Send(new GetChannelMessageRequest(user.Id, messageId));
            var channelId = channelMessage.Message.ChannelId;
            var client = chatHubContext.Clients.Group("channel-" + channelId);

            await ChatHubSender.MarkMessageRead(client, channelId, messageId, user);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception) { }
    }
}