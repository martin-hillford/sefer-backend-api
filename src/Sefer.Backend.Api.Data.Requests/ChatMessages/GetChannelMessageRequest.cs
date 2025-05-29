namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class GetChannelMessageRequest(int userId, int messageId) : IRequest<ChannelMessage>
{
    public readonly int UserId = userId;

    public readonly int MessageId = messageId;
}