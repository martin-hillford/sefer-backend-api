namespace Sefer.Backend.Api.Data.Requests.Channels;

public class CreateGroupChannelRequest(int mentor, string name) : IRequest<Channel>
{
    public readonly int Mentor = mentor;

    public readonly string Name = name;
}