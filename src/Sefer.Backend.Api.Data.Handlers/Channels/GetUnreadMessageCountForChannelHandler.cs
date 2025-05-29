namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class GetUnreadMessageCountForChannelHandler(IServiceProvider serviceProvider)
    : Handler<GetUnreadMessageCountForChannelRequest, int>(serviceProvider)
{
    public override async Task<int> Handle(GetUnreadMessageCountForChannelRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.ChatChannelMessages
            .AsNoTracking()
            .CountAsync(c =>
                c.Message.ChannelId == request.ChannelId &&
                c.ReceiverId == request.UserId &&
                c.ReadDate == null,
                token
            );
    }
}