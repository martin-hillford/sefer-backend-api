namespace Sefer.Backend.Api.Data.Requests.Channels;

public class GetUnreadMessageCountRequest(int userId) : IRequest<Dictionary<int, int>>
{
    public readonly int UserId = userId;
}