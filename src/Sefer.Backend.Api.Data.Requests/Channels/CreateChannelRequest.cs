namespace Sefer.Backend.Api.Data.Requests.Channels;

public class CreateChannelRequest(int userA, int userB) : IRequest<Channel>
{
    public readonly int UserA = userA;

    public readonly int UserB = userB;
}