namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class GetTotalUnreadMessagesRequest(int userId) : IRequest<int>
{
    public readonly int UserId = userId;
}