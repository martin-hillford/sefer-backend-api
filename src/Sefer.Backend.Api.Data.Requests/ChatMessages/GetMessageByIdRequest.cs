namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class GetMessageByIdRequest(int userId, int messageId) : IRequest<Message>
{
    public readonly int UserId = userId;

    public readonly int MessageId = messageId;
}