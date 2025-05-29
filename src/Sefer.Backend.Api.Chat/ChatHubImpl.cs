namespace Sefer.Backend.Api.Chat;

internal class ChatHubImpl(IMediator mediator, IChatHubContext context, INotificationService notificationService)
{
    #region Invokable methods

    internal async Task JoinChannel(int channelId)
    {
        // check the user and see that he is in the channel
        var userId = GetUserId();
        var isInChannel = await IsUserInChannel(userId, channelId);
        if (!isInChannel) return;

        // reply to the user by sending him a message with all the user in the channel
        var users = await mediator.Send(new GetUsersInChannelRequest(channelId));
        var view = new { Users = users.Select(u => new PrimitiveUserView(u)), ChannelId = channelId };
        await SendAsync("onJoinChannel", view);

        // Now join the signal-r group
        await context.GetGroups().AddToGroupAsync(context.GetConnectionId(), "channel-" + channelId);

        // Also send to all the groups members that the new user is online
        var clients = context.GetClients();
        var tasks = users
            .Where(u => u.Id != userId)
            .Select(u => clients.User(u.Id.ToString()).SendAsync("onReportedChannelState", view));
        await Task.WhenAll(tasks);
    }

    internal async Task LeaveChannel(int channelId)
    {
        // check the user and see that he is in the channel
        var user = await GetUser();
        var isInChannel = await IsUserInChannel(user?.Id, channelId);
        if (!isInChannel || user == null) return;
        
        // Get the context of this chat
        var groups = context.GetGroups();
        var connectionId = context.GetConnectionId();
        var clients = context.GetClients();
        
        // Now leave the signal-r group
        await groups.RemoveFromGroupAsync(connectionId, "channel-" + channelId);

        // Also send to all the groups members that the user has left
        var users = await mediator.Send(new GetUsersInChannelRequest(channelId));
        var view = new { UserId = user.Id, ChannelId = channelId, UserName = user.Name, State = "offline" };
        var tasks = users
            .Where(u => u.Id != user.Id)
            .Select(u => clients.User(u.Id.ToString()).SendAsync("onReportedChannelState", view));
        await Task.WhenAll(tasks);
    }

    internal async Task WhoIsInChannel(int channelId)
    {
        // check the user and see that he is in the channel
        var userId = GetUserId();
        var isInChannel = await IsUserInChannel(userId, channelId);
        if (!isInChannel) return;
        
        var clients = context.GetClients();
        var view = new { UserId = userId, ChannelId = channelId };
        await clients.Group("channel-" + channelId).SendAsync("onReportChannelState", view);
    }

    internal async Task ReportChannelState(int channelId, string state, int receiverId)
    {
        // check the user and see that he is in the channel
        var user = await GetUser();
        var isInChannel = await IsUserInChannel(user?.Id, channelId);
        if (!isInChannel || user == null) return;
        
        // check for the user and if he is in the channel
        var receiverUser = await mediator.Send(new GetUserByIdRequest(receiverId));
        if (receiverUser == null) return;

        // Also send to all the groups members that the new user is online
        var clients = context.GetClients();
        var view = new { UserId = user.Id, ChannelId = channelId, UserName = user.Name, State = state };
        await clients.User(receiverId.ToString()).SendAsync("onReportedChannelState", view);
    }

    internal async Task SendMessage(SendingMessage message)
    {
        var userId = GetUserId();
        var isInChannel = await IsUserInChannel(userId, message.ChannelId);
        if (!isInChannel || !userId.HasValue) { await SendAsync("onMessageSend", new { Result = "not-in-channel", message.TempId }); return; }

        // post the message into the channel
        var request = new PostTextChatMessageRequest
        {
            Text = message.Content,
            ChannelId = message.ChannelId,
            SenderId = userId.Value,
            QuotedMessageId = message.QuotedMessageId,
            QuotedMessageText = message.QuotedMessage
        };
        var result = await mediator.Send(request);

        // Notify the sender of the message the result of sending the message
        if (result == null) { await SendAsync("onMessageSend", new { Result = "message-not-send", message.TempId }); return; }
        await SendAsync("onMessageSend", new { Result = "success", message.TempId, MessageId = result.Id });

        // Here is the thing, we could easily use groups send functionality. But that will only send the message to
        // users that are actually online in the channel. We want to send the message to all the people that are online
        // and have access to the channel

        // However, since some user will have notifications enabled, either on their device or by e-mail,
        // the notification service is used to deal with. As a consequence, this method will to send the message itself
        // to the websocket!
        var tasks = result.ChannelMessages.Select(channelMsg => notificationService.SendChatMessageSendNotificationAsync(channelMsg, true));
        await Task.WhenAll(tasks);
    }

    internal async Task MessagesRead(int channelId, int messageId)
    {
        var user = await GetUser();
        if (user == null) return;

        var marked = await mediator.Send(new MarkMessageAsReadRequest(messageId, user.Id));
        if (!marked) return;
        
        var clients = context.GetClients();
        var client = clients.Group("channel-" + channelId);
        await ChatHubSender.MarkMessageRead(client, channelId, messageId, user);
    }

    internal async Task Typing(int channelId)
    {
        // check the user and see that he is in the channel
        var userId = GetUserId();
        var isInChannel = await IsUserInChannel(userId, channelId);
        if (!isInChannel) return;
        
        var clients = context.GetClients();
        var view = new { UserId = userId, ChannelId = channelId };
        await clients.Group("channel-" + channelId).SendAsync("onTyping", view);
    }

    #endregion

    #region Private Methods

    private int? GetUserId()
    {
        var identifier = context.GetUserIdentifier();
        if (int.TryParse(identifier, out var userId) == false) return null;
        return userId;
    }
    
    private async Task<User> GetUser()
    {
        var userId = GetUserId();
        if(userId == null) return null;
        return await mediator.Send(new GetUserByIdRequest(userId));
    }

    private async Task SendAsync(string method, object message = null)
    {
        var clients = context.GetClients();
        if (message == null) await clients.Caller.SendAsync(method);
        else await clients.Caller.SendAsync(method, message);
    }

    private async Task<bool> IsUserInChannel(int? userId, int? channelId)
    {
        if(userId == null || channelId == null) return false;
        return await mediator.Send(new IsUserInChannelRequest(channelId.Value, userId.Value));
    }

    #endregion
}