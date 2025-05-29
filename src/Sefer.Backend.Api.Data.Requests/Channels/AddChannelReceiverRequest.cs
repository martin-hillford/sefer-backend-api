namespace Sefer.Backend.Api.Data.Requests.Channels;

public class AddChannelReceiverRequest(int channelId, List<int> students) : IRequest<Channel>
{
    public readonly int ChannelId = channelId;

    public readonly List<int> Students = students;
}