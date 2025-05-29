namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class PostNameChangeChatMessageRequest(string oldName, int userId) : IRequest<List<Message>>
{
    public readonly string OldName = oldName;

    public readonly int UserId = userId;
}