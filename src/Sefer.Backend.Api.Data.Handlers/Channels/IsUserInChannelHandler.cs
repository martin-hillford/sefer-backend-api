namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class IsUserInChannelHandler(IServiceProvider serviceProvider)
    : Handler<IsUserInChannelRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsUserInChannelRequest request, CancellationToken token)
    {
        // Note: in the past the not deleted check was not present.
        var context = GetDataContext();
        return await context.ChatChannelReceivers
            .AsNoTracking()
            .AnyAsync(c => c.ChannelId == request.ChannelId && c.UserId == request.UserId && !c.Deleted, token);
    }
}