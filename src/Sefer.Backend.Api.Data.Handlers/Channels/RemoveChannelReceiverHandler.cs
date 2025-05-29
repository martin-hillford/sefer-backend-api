namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class RemoveChannelReceiverHandler(IServiceProvider serviceProvider)
    : ChannelReceiverHandler<RemoveChannelReceiverRequest, Channel>(serviceProvider)
{
    public override async Task<Channel> Handle(RemoveChannelReceiverRequest request, CancellationToken token)
    {
        // Check if the channel exists
        var channel = await GetChannel(request.ChannelId, token);
        if (channel == null) return null;
        
        // Check if the student is in channel or not
        var current = await GetChannelReceivers(request.ChannelId, token);
        var removing = request.Students.Intersect(current).ToList();
        if (removing.Count == 0) return channel;

        await using var context = GetDataContext();
        var toRemove = new List<ChannelReceiver>();
        foreach (var student in removing)
        {
            var receiver = await context.ChatChannelReceivers
                .SingleOrDefaultAsync(c => c.ChannelId == request.ChannelId && c.UserId == student, cancellationToken: token);
            if (receiver != null) toRemove.Add(receiver);   
        }
        
        // Remove the student from the channel and return the new channel information
        context.ChatChannelReceivers.RemoveRange(toRemove);
        await context.SaveChangesAsync(token);
        return await GetChannel(request.ChannelId, token);
    }
}