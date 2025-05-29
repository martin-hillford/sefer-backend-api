namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class MarkChannelAsReadHandler(IServiceProvider serviceProvider)
    : Handler<MarkChannelAsReadRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(MarkChannelAsReadRequest request, CancellationToken token)
    {
        try
        {
            var inChannel = await Send(new IsUserInChannelRequest(request.ChannelId, request.UserId), token);
            if (!inChannel) return false;

            var now = DateTime.UtcNow;
            await using var context = GetDataContext();
            if (context.Database.IsSqlServer())
            {
                await context.ChatChannelMessages
                    .Where(m => m.ReadDate == null && m.ReceiverId == request.ChannelId && m.Message.ChannelId == request.ChannelId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(r => r.ReadDate, now), token);
            }
            else
            {
                var messages = await context.ChatChannelMessages.Where(c => c.ReadDate == null && c.Message.ChannelId == request.ChannelId && c.ReceiverId == request.UserId).ToListAsync(token);
                messages.ForEach(m => { m.ReadDate = DateTime.UtcNow; });
                context.UpdateRange(messages);
                await context.SaveChangesAsync(token);
            }
        }
        catch (Exception) { return false; }
        return true;
    }
}