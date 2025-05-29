namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class GetTotalUnreadMessagesHandler(IServiceProvider serviceProvider)
    : Handler<GetTotalUnreadMessagesRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetTotalUnreadMessagesRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ChatChannelMessages
            .AsNoTracking()
            .CountAsync(c => c.ReadDate.HasValue == false && c.Message.SenderId !=request.UserId && c.ReceiverId == request.UserId, token);
    }
}