namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class PostNameChangeChatMessageHandler(IServiceProvider serviceProvider)
    : PostChatMessageHandler<PostNameChangeChatMessageRequest, List<Message>>(serviceProvider)
{
    public override async Task<List<Message>> Handle(PostNameChangeChatMessageRequest request, CancellationToken token)
    {
        var messages = new List<Message>();

        if (string.IsNullOrEmpty(request.OldName?.Trim())) return messages;
        var channels = await Send(new GetChannelsRequest(request.UserId), token);

        foreach (var channel in channels)
        {
            var message = await Post(request.OldName, request.UserId, channel.Id, MessageTypes.NameChange, token);
            if (message != null) messages.Add(message);
        }

        return messages;
    }
}