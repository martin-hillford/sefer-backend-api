namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class PostTextChatMessageHandler(IServiceProvider serviceProvider)
    : PostChatMessageHandler<PostTextChatMessageRequest, Message>(serviceProvider)
{
    public override async Task<Message> Handle(PostTextChatMessageRequest request, CancellationToken token)
    {
        var (text, quotedMessageId, quotedMessageText, channelId, userId) = request.GetParams();
        var message = await Post(text, quotedMessageId, quotedMessageText, userId, channelId, MessageTypes.Text, token);
        if (message == null) return null;

        // Query for the messages that are in the scope of the request
        await using var context = GetDataContext();
        return await context.ChatMessages
            .AsNoTracking()
            .Where(m => m.Id == message.Id)
            .Include(m => m.Sender)
            .Include(m => m.Channel)
            .Include(m => m.ChannelMessages).ThenInclude(c => c.Receiver)
            .Include(m => m.QuotedMessage).ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(token);
    }
}