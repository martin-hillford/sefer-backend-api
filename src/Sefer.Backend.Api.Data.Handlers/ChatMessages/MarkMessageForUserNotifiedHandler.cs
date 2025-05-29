namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class MarkMessageForUserNotifiedHandler(IServiceProvider serviceProvider)
    : SyncHandler<MarkMessageForUserNotifiedRequest, bool>(serviceProvider)
{
    protected override bool Handle(MarkMessageForUserNotifiedRequest request)
    {
        try
        {
            var context = GetDataContext();
            var channelMessages = context.ChatChannelMessages
                .Where(m => m.MessageId == request.MessageId && m.ReceiverId == request.UserId)
                .ToList();
            if (channelMessages.Count == 0) return false;

            foreach (var message in channelMessages) { message.IsNotified = true; }
            context.ChatChannelMessages.UpdateRange(channelMessages);
            context.SaveChanges();
            context.Dispose();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}