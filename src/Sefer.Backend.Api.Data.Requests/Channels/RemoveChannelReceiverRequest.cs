namespace Sefer.Backend.Api.Data.Requests.Channels;

public class RemoveChannelReceiverRequest(int channelId, List<int> students) : IRequest<Channel>
{
    public readonly int ChannelId = channelId;

    public readonly List<int> Students = students;
}