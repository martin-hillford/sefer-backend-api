namespace Sefer.Backend.Api.Data.Requests.Channels;

public class MarkMessagesAsReadRequest(int userId, int channelId, HashSet<int> messageIds) : IRequest<bool>
{
    public readonly int UserId = userId;

    public readonly int ChannelId = channelId;

    public readonly HashSet<int> MessageIds = messageIds;
}