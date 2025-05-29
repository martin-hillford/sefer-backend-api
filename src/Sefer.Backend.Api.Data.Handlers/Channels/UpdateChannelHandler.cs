namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class UpdateChannelHandler(IServiceProvider serviceProvider)
    : Handler<UpdateChannelRequest, Channel>(serviceProvider)
{
    public override async Task<Channel> Handle(UpdateChannelRequest request, CancellationToken token)
    {
        // 1) Get Channel, if that does not exist return null
        var channel = await GetChannel(request, token);
        if(channel == null) return null;
        
        // 2) Validate the name of the channel
        var valid = await ValidateChannel(request, token);
        if(!valid) return null;
        
        // 2) Now determine which students to remove from the channel and remove them
        var toRemove = channel.Receivers.Where(r => r.UserId != request.MentorId && !request.Students.Contains(r.UserId));
        foreach (var receiver in toRemove) { await DeleteStudent(channel, receiver.UserId, token); }

        // 3) Now determine which students to add to channel and add them
        var studentsToAdd = request.Students.Where(s => channel.Receivers.All(r => r.UserId != s)).ToList();
        await AddStudents(channel, studentsToAdd, token);
        
        // 4) update the name of the channel
        await UpdateChannelName(request, token);

        // 5) Now reload the channel data and return
        return await GetChannel(request, token);
    }

    private async Task DeleteStudent(Channel channel, int studentId, CancellationToken token)
    {
        await using var context = GetDataContext();
        var receiver = context.ChatChannelReceivers.SingleOrDefault(r => r.UserId == studentId && r.ChannelId == channel.Id);
        if(receiver == null) return;
        
        var chatMetadata = context.ChatChannelMessages.Where(c => c.ReceiverId== receiver.Id);
        context.ChatChannelMessages.RemoveRange(chatMetadata);
        await context.SaveChangesAsync(token);    
        
        context.ChatChannelReceivers.Remove(receiver);
        await context.SaveChangesAsync(token);
    }

    private async Task AddStudents(Channel channel, List<int> students, CancellationToken token)
    {
        // Create a new receiver object for this student
        await using var context = GetDataContext();
        
        // Create a list of id of all the messages in this channel
        var messages = await context.ChatMessages.Where(c => c.ChannelId == channel.Id).Select(m => m.Id).ToListAsync(token);
        
        // Create for all the students a receiver object in the database
        var receivers = students.Select(s => new ChannelReceiver { UserId = s, ChannelId = channel.Id, HasPostRights = true, CreationDate = DateTime.UtcNow }).ToList();
        context.ChatChannelReceivers.AddRange(receivers);
        await context.SaveChangesAsync(token);
        
        // Create for all the receivers the message metadata
        // Be aware that the ReceiverId for ChannelMessage is actually the userId!
        var metadata = receivers.SelectMany(rec => messages.Select(m => new ChannelMessage { MessageId = m, ReceiverId = rec.UserId, IsNotified = true }));
        context.ChatChannelMessages.AddRange(metadata);
        await context.SaveChangesAsync(token);
    }
    
    private async Task<Channel> GetChannel(UpdateChannelRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var channel = await context.ChatChannels
            .Include(c => c.Receivers)
            .SingleOrDefaultAsync(c => c.Id == request.ChannelId, token);
        return channel;
    }

    private async Task UpdateChannelName(UpdateChannelRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var channel = await context.ChatChannels.SingleOrDefaultAsync(c => c.Id == request.ChannelId, token);
        channel.Name = request.Name;
        await context.SaveChangesAsync(token);
    }

    private async Task<bool> ValidateChannel(UpdateChannelRequest request, CancellationToken token)
    {
        // Check if the name of the channel already exists
        var channels = await Send(new GetChannelsRequest(request.MentorId), token);
        var filtered = channels.Where(c => c.Name?.Trim().ToLower() == request.Name.Trim().ToLower() && c.Type == ChannelTypes.Private).ToList();
        return filtered.Count == 0 || filtered.All(c => c.Id == request.ChannelId);
    }
}