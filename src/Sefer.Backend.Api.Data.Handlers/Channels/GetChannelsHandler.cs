namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class GetChannelsHandler(IServiceProvider serviceProvider)
    : Handler<GetChannelsRequest, List<Channel>>(serviceProvider)
{
    public override async Task<List<Channel>> Handle(GetChannelsRequest request, CancellationToken token)
    {
        // Query for personal channels the user has access to and sort then last message date
        // such that the channel with the latest message is on top
        await using var context = GetDataContext();
        if (!context.Database.IsSqlCapableServer()) return await PureLinq(context, request, token);
        return await SqlServer(context, request, token);
    }

    private static async Task<List<Channel>> PureLinq(DataContext context, GetChannelsRequest request, CancellationToken token)
    {
        return await context.ChatChannels
            .AsNoTracking()
            .Where(c =>
                c.Type != ChannelTypes.Public &&
                c.Receivers.Any(r => r.UserId == request.UserId && r.Deleted == false && r.Archived == false)
            )
            .Include(c => c.Receivers).ThenInclude(r => r.User)
            .OrderByDescending(c => c.Messages.Max(m => m.SenderDate))
            .ToListAsync(token);
    }

    private static async Task<List<Channel>> SqlServer(DataContext context, GetChannelsRequest request, CancellationToken token)
    {
        var userId = request.UserId;
        var query = $@"
            SELECT c.* FROM chat_channels AS c
            LEFT JOIN ( SELECT cm.channel_id, MAX(sender_date) AS sender_date FROM chat_messages as cm GROUP BY cm.channel_id ) AS cs ON cs.channel_id = C.id
            WHERE c.id IN ( SELECT DISTINCT channel_id FROM chat_channel_receivers WHERE deleted = FALSE AND archived = FALSE AND user_id = {userId} ) AND type != 2
            ORDER BY sender_date DESC
        ";

        var channels = await context.ChatChannels.FromSqlRaw(query, userId).ToDictionaryAsync(c => c.Id, token);
        var receivers = await context.ChatChannels
            .AsNoTracking()
            .Where(c => c.Receivers.Any(r => r.UserId == request.UserId && r.Deleted == false && r.Archived == false))
            .Include(c => c.Receivers).ThenInclude(r => r.User)
            .SelectMany(c => c.Receivers)
            .ToListAsync(token);
        
        foreach (var receiver in receivers)
        {
            if (!channels.TryGetValue(receiver.ChannelId, out var channel)) continue;
            channel.Receivers ??= new List<ChannelReceiver>();
            channel.Receivers.Add(receiver);
        }

        return channels.Values.ToList();
    }
}