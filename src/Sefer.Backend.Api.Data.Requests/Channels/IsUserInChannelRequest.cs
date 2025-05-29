namespace Sefer.Backend.Api.Data.Requests.Channels;

public class IsUserInChannelRequest(int channelId, int userId) : IRequest<bool>
{
    public readonly int ChannelId = channelId;
    
    public readonly int UserId = userId;
}