namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class MarkMessagesAsReadHandler(IServiceProvider serviceProvider)
    : SyncHandler<MarkMessagesAsReadRequest, bool>(serviceProvider)
{
    protected override bool Handle(MarkMessagesAsReadRequest request)
    {
        try
        {
            using var context = GetDataContext();
            var messages = context.ChatChannelMessages
                .Where(c =>
                    request.MessageIds.Contains(c.MessageId) &&
                    c.Message.ChannelId == request.ChannelId &&
                    c.ReceiverId == request.UserId &&
                    c.ReadDate == null
                )
                .ToList();

            foreach (var message in messages)
            {
                message.ReadDate = DateTime.UtcNow;
                context.ChatChannelMessages.Update(message);
            }

            context.SaveChanges();
            return true;
        }
        catch (Exception) { return false; }
    }
}