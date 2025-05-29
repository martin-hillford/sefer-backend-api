namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class GetUnreadMessageCountHandler(IServiceProvider serviceProvider)
    : Handler<GetUnreadMessageCountRequest, Dictionary<int, int>>(serviceProvider)
{
    public override async Task<Dictionary<int, int>> Handle(GetUnreadMessageCountRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ChatChannelMessages
            .AsNoTracking()
            .Where(cm => cm.ReceiverId == request.UserId && cm.ReadDate == null && cm.Message.SenderId != request.UserId)
            .GroupBy(cm => cm.Message.ChannelId)
            .Select(cm => new { ChannelId = cm.Key, UnreadMessageCount = cm.Count() })
            .ToDictionaryAsync(cm => cm.ChannelId, cm => cm.UnreadMessageCount, token);
    }
}