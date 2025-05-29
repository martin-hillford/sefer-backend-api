namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class GetPersonalChannelHandler(IServiceProvider serviceProvider)
    : Handler<GetPersonalChannelRequest, Channel>(serviceProvider)
{
    public override async Task<Channel> Handle(GetPersonalChannelRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var channel = await context.ChatChannels
            .AsNoTracking()
            .Where(c => c.Type == ChannelTypes.Personal &&
                        c.Receivers.All(r => r.UserId == request.UserA || r.UserId == request.UserB) &&
                        c.Receivers.Count == 2)
            .Include(c => c.Receivers)
            .FirstOrDefaultAsync(token);

        if (channel != null) return channel;
        return await Send(new CreateChannelRequest(request.UserA, request.UserB), token);
    }
}