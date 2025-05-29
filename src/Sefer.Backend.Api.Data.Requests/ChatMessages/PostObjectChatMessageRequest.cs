namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class PostObjectChatMessageRequest : IRequest<Message>
{
    public int ReferenceId { get; init; }

    public object Message { get; init; }

    public int SenderId { get; init; }

    public int ChannelId { get; init; }
    
    public MessageTypes Type { get; init; }
}
