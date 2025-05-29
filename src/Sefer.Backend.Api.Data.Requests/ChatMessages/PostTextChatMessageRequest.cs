namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class PostTextChatMessageRequest : IRequest<Message>
{
    public string Text  { get; init; }

    public int? QuotedMessageId { get; init; }

    public string QuotedMessageText { get; init; }
    
    public int SenderId { get; init; }

    public int ChannelId { get; init; }
}