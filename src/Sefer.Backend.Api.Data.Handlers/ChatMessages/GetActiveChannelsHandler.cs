namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class GetActiveChannelsHandler(IServiceProvider serviceProvider)
    : Handler<GetActiveChannelsRequest, Dictionary<int, UserLastActivity>>(serviceProvider)
{
    public override async Task<Dictionary<int, UserLastActivity>> Handle(GetActiveChannelsRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();

        // Create a subquery of all the users within channels of the mentor
        var receiverIds = context.ChatChannels
            .Where(c => c.Type == ChannelTypes.Personal && c.Receivers.Any(r => r.UserId == request.MentorId && !r.Deleted))
            .SelectMany(c => c.Receivers.Select(r => r.UserId));

        // Now load the activity of those users
        return await context.UserLastActivities.Where(u => receiverIds.Contains(u.UserId)).ToDictionaryAsync(c => c.UserId, token);
    }
}