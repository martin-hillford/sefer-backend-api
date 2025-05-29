namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class CreateChannelHandler(IServiceProvider serviceProvider)
    : Handler<CreateChannelRequest, Channel>(serviceProvider)
{
    public override async Task<Channel> Handle(CreateChannelRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var now = DateTime.UtcNow;

        // Create first a chanel
        var channel = new Channel { CreationDate = now, Type = ChannelTypes.Personal };
        context.ChatChannels.Add(channel);
        await context.SaveChangesAsync(token);

        // Add the users as receivers
        var receivers = new List<ChannelReceiver>
        {
            new() { CreationDate = now, ChannelId = channel.Id, HasPostRights = true, UserId = request.UserA },
            new() { CreationDate = now, ChannelId = channel.Id, HasPostRights = true, UserId = request.UserB }
        };
        context.ChatChannelReceivers.AddRange(receivers);
        await context.SaveChangesAsync(token);
        await context.DisposeAsync();

        // And return the channel
        channel.Receivers = receivers;
        return channel;
    }
}