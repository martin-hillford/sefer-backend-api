namespace Sefer.Backend.Api.Data.Requests.Channels;

public class GetUnreadMessageCountForChannelRequest(int userId, int channelId) : IRequest<int>
{
    public readonly int UserId = userId;

    public readonly int ChannelId = channelId;
}