namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class GetChatMessagesHandler(IServiceProvider serviceProvider)
    : Handler<GetChatMessagesRequest, List<Message>>(serviceProvider)
{
    public override async Task<List<Message>> Handle(GetChatMessagesRequest request, CancellationToken token)
    {
        // Check if the can load something at all
        if (request.Take <= 0) return [];

        // Query for the messages that are in the scope of the request
        await using var context = GetDataContext();
        var query = context.ChatMessages.Where(m => m.ChannelId == request.ChannelId && m.IsAvailable).OrderByDescending(m => m.SenderDate).Skip(request.Skip).Take(request.Take);
        var messages = await query.ToListAsync(token);

        await query.Include(m => m.ChannelMessages).LoadAsync(token);
        await query.Include(m => m.QuotedMessage).LoadAsync(token);

        // Since joining on all the in the message will take a lot of time. this is done separately
        var users = await GetChatUsers(messages, request.ChannelId, token);
        return messages.Select(m => Merge(m, users)).ToList();
    }

    private async Task<Dictionary<int, User>> GetChatUsers(List<Message> messages, int channelId, CancellationToken token)
    {
        // Because of the (private) chat groups it can happen that a user is no longer part of the channel
        // but that are still messages of him in the channel. So this method will load all the user information
        
        // First load all the receivers in the channel
        await using var context = GetDataContext();
        var receivers = await context.ChatChannelReceivers
            .Where(c => c.ChannelId == channelId)
            .Select(c => c.User)
            .ToDictionaryAsync(r => r.Id, token);
        
        // Next load all ids of users that can be found in the message
        var missing =
            messages.Select(m => m.SenderId)
                .Union(messages.Where(m => m.QuotedMessage != null).Select(m => m.QuotedMessage.SenderId))
                .Union(messages.SelectMany(m => m.ChannelMessages).Select(c => c.ReceiverId))
                .Distinct()
                .Except(receivers.Keys)
                .ToList();

        // The missing now contains all the users that have not been load
        if (missing.Count == 0) return receivers;
        var users = await context.Users.Where(u => missing.Contains(u.Id)).ToDictionaryAsync(u => u.Id, token);
        return receivers.Merge(users);
    }
    
    private static Message Merge(Message message, Dictionary<int, User> users)
    {
        message.Sender = users[message.SenderId];
        var channelMessages = new List<ChannelMessage>();
        
        // Note: the receiverId and senderId of a channelMessage does NOT refer to a channelReceiver but a user
        foreach (var channelMessage in message.ChannelMessages)
        {
            if (!users.TryGetValue(channelMessage.ReceiverId, out var user)) continue;
            channelMessage.Receiver = user;
            channelMessages.Add(channelMessage);
        }
        
        message.ChannelMessages = channelMessages;
        return message;
    }
}