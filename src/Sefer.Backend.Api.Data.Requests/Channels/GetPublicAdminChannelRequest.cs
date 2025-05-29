namespace Sefer.Backend.Api.Data.Requests.Channels;

public class GetPublicAdminChannelRequest(int user) : IRequest<Channel>
{
    public readonly int User = user;
}