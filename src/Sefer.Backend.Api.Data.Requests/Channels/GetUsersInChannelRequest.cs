namespace Sefer.Backend.Api.Data.Requests.Channels;

public class GetUsersInChannelRequest(int channelId) : IRequest<List<User>>
{
    public readonly int ChannelId = channelId;
}