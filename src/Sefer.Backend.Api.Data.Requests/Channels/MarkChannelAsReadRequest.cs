namespace Sefer.Backend.Api.Data.Requests.Channels;

public class MarkChannelAsReadRequest(int userId, int channelId) : IRequest<bool>
{
    public readonly int UserId = userId;

    public readonly int ChannelId = channelId;
}