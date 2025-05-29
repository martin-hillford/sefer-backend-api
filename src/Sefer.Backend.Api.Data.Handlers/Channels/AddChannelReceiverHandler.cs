namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class AddChannelReceiverHandler(IServiceProvider serviceProvider)
    : ChannelReceiverHandler<AddChannelReceiverRequest, Channel>(serviceProvider)
{
    public override async Task<Channel> Handle(AddChannelReceiverRequest request, CancellationToken token)
    {
        // Check if the channel exists
        var channel = await GetChannel(request.ChannelId, token);
        if (channel == null) return null;
        
        // Check if the student is already a receiver in the channel
        var current = await GetChannelReceivers(request.ChannelId, token);
        var missing = request.Students.Except(current).ToList();
        if (missing.Count == 0) return channel;
        
        // If this is not the case add the student to the channel
        await using var context = GetDataContext();
        var receivers = missing.Select(s => Create(channel.Id, s)).ToList();
        context.ChatChannelReceivers.AddRange(receivers);
        await context.SaveChangesAsync(token);
        
        // And return the new channel information
        return await GetChannel(request.ChannelId, token);
    }

    private static ChannelReceiver Create(int channelId, int studentId)
        =>  new ChannelReceiver
            { CreationDate = DateTime.Now, ChannelId = channelId, HasPostRights = true, UserId = studentId };
}