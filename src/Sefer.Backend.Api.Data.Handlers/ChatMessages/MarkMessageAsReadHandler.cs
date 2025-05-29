namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class MarkMessageAsReadHandler(IServiceProvider serviceProvider)
    : Handler<MarkMessageAsReadRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(MarkMessageAsReadRequest request, CancellationToken token)
    {
        try
        {
            await using var context = GetDataContext();
            var channelMessages = await context.ChatChannelMessages
                .Where(m => m.MessageId == request.MessageId && m.ReceiverId == request.UserId)
                .ToListAsync(token);

            // Because of a bug during development there the possibility that
            // a channel message is not present for the user, thus it is created
            if (channelMessages.Count == 0)
            {
                var channelMessage = Create(request);
                context.ChatChannelMessages.Add(channelMessage);
            }
            else
            {
                channelMessages.ForEach(message => { message.ReadDate = DateTime.Now; });
                context.ChatChannelMessages.UpdateRange(channelMessages);                
            }
            
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static ChannelMessage Create(MarkMessageAsReadRequest request)
    {
        return new ChannelMessage
        {
            MessageId = request.MessageId,
            ReceiverId = request.UserId,
            IsNotified = true,
            ReadDate = DateTime.UtcNow,
            IsMarked = true
        };
    }
}