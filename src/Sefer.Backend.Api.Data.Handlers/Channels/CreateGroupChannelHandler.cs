namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class CreateGroupChannelHandler(IServiceProvider serviceProvider)
    : Handler<CreateGroupChannelRequest, Channel>(serviceProvider)
{
    public override async Task<Channel> Handle(CreateGroupChannelRequest request, CancellationToken token)
    {
        // Check if the provider user 
        await using var context = GetDataContext();
        var mentor = await context.Users.SingleAsync(u => u.Id == request.Mentor, token);
        if (mentor?.IsMentor != true) return null;
        
        // Create the group / private channel
        var now = DateTime.UtcNow;
        var channel = new Channel { CreationDate = now, Type = ChannelTypes.Private, Name = request.Name };
        context.ChatChannels.Add(channel);
        await context.SaveChangesAsync(token);
        
        // Add the mentor as the receiver
        var receivers = new List<ChannelReceiver>
        {
            new() { CreationDate = now, ChannelId = channel.Id, HasPostRights = true, UserId = request.Mentor }
        };
        context.ChatChannelReceivers.AddRange(receivers);
        await context.SaveChangesAsync(token);

        // And return the channel
        channel.Receivers = receivers;
        return channel;
    }
}