namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class MarkMessageForUserNotifiedRequest(int messageId, int userId) : IRequest<bool>
{
    public readonly int MessageId = messageId;
    
    public readonly int UserId = userId;
}