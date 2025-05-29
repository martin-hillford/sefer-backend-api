namespace Sefer.Backend.Api.Data.Handlers.Channels;

public class DeletePrivateChannelHandler(IServiceProvider serviceProvider) : Handler<DeletePrivateChannelRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(DeletePrivateChannelRequest request, CancellationToken token)
    {
        // First check if the given mentor is present in the list of receiver and is a mentor
        var context = GetDataContext();
        var isPresent = await context.ChatChannelReceivers
            .AsNoTracking()
            .AnyAsync(c => c.ChannelId == request.ChannelId && c.UserId == request.MentorId && !c.Deleted, token);
        var isMentor = await context.Users.AsNoTracking()
            .AnyAsync(u => u.Id == request.MentorId && u.Role == UserRoles.Mentor, token);
        if (!isPresent || !isMentor) return false;
        
        try
        {
            await context.ChatChannelMessages.Where(s => s.Message.ChannelId == request.ChannelId).ExecuteDeleteAsync(token);
            await context.ChatChannelReceivers.Where(s => s.ChannelId == request.ChannelId).ExecuteDeleteAsync(token);
            await context.ChatMessages.Where(s => s.ChannelId == request.ChannelId).ExecuteDeleteAsync(token);
            await context.ChatChannels.Where(c => c.Id == request.ChannelId).ExecuteDeleteAsync(token);
            return true;
        }
        catch (Exception) {  return false;  }
    }
}