namespace Sefer.Backend.Api.Data.Handlers.Channels;

public abstract class ChannelReceiverHandler<TRequest, TResponse>(IServiceProvider serviceProvider)
    : Handler<TRequest, TResponse>(serviceProvider)
    where TRequest : IRequest<TResponse>
{
    protected async Task<Channel> GetChannel(int channelId, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ChatChannels
            .AsNoTracking()
            .Where(c => c.Id == channelId)
            .Include(c => c.Receivers)
            .FirstOrDefaultAsync(token);
    }

    protected async Task<List<int>> GetChannelReceivers(int channelId, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ChatChannelReceivers
            .AsNoTracking()
            .Where(c => c.ChannelId == channelId)
            .Select(c => c.UserId)
            .ToListAsync(token);
    }
}