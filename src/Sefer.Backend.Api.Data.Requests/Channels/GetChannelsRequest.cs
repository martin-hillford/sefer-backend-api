namespace Sefer.Backend.Api.Data.Requests.Channels;

public class GetChannelsRequest(int userId) : IRequest<List<Channel>>
{
    public readonly int UserId = userId;
}