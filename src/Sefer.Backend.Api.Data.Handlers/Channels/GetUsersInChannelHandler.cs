namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class GetUsersInChannelHandler(IServiceProvider serviceProvider)
    : Handler<GetUsersInChannelRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetUsersInChannelRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ChatChannelReceivers
            .AsNoTracking()
            .Where(c => c.ChannelId == request.ChannelId && c.Deleted == false)
            .Select(c => c.User)
            .ToListAsync(token);
    }
}