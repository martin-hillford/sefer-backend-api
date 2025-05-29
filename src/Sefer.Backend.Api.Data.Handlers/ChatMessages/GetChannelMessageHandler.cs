namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class GetChannelMessageHandler(IServiceProvider serviceProvider)
    : Handler<GetChannelMessageRequest, ChannelMessage>(serviceProvider)
{
    public override async Task<ChannelMessage> Handle(GetChannelMessageRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ChatChannelMessages
            .AsNoTracking()
            .Include(c => c.Message)
            .FirstOrDefaultAsync(m => m.ReceiverId == request.UserId && m.MessageId == request.MessageId, token);
    }
}