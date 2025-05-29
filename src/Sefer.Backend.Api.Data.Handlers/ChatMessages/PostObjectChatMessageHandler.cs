namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class PostObjectChatMessageHandler(IServiceProvider serviceProvider)
    : Handler<PostObjectChatMessageRequest, Message>(serviceProvider)
{
    public override async Task<Message> Handle(PostObjectChatMessageRequest request, CancellationToken token)
    {
        try { return await HandleRequest(request, token); }
        catch (Exception) { return null; }
    }

    private async Task<Message> HandleRequest(PostObjectChatMessageRequest request, CancellationToken token)
    {
        // Check the input
        if (request.ReferenceId < 1) return null;
        var now = DateTime.UtcNow;

        // Check if the user in channel
        if (!await Send(new IsUserInChannelRequest(request.ChannelId, request.SenderId), token)) return null;
        var channel = await Send(new GetChannelByIdRequest(request.ChannelId), token);
        if (channel == null) return null;

        // Insert the message
        var message = new Message
        {
            ChannelId = request.ChannelId,
            SenderId = request.SenderId,
            SenderDate = now,
            Type = request.Type,
            ReferenceId = request.ReferenceId,
            ContentString = JsonSerializer.Serialize(request.Message, JsonOptions.Options)
        };
        if (!await IsValidAsync(message)) return null;

        await using var context = GetDataContext();
        context.ChatMessages.Add(message);
        await context.SaveChangesAsync(token);

        // Insert all the metadata for each user in the channel
        var receivers = context.ChatChannelReceivers
            .Where(r => r.ChannelId == request.ChannelId)
            .Include(r => r.User)
            .ToList();

        foreach (var receiver in receivers)
        {
            var meta = new ChannelMessage { MessageId = message.Id, ReceiverId = receiver.UserId };
            if (receiver.UserId == request.SenderId) meta.ReadDate = now;
            context.ChatChannelMessages.Add(meta);
        }

        // The Message is inserted, the content added and the receivers send, save it and done.
        await context.SaveChangesAsync(token);

        message.IsAvailable = true;
        context.ChatMessages.Update(message);
        await context.SaveChangesAsync(token);

        // Once it is saved add the proper references.
        message.ChannelMessages = context.ChatChannelMessages.Where(m => m.MessageId == m.Id).Include(r => r.Receiver).ToList();
        message.Channel = channel;
        
        return message;
    }
}