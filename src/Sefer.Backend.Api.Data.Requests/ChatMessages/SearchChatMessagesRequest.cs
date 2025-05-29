namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class SearchChatMessagesRequest(int userId, string searchTerm) : IRequest<List<ChatSearchResult>>
{
    public readonly int UserId = userId;

    public readonly string SearchTerm = searchTerm;
}