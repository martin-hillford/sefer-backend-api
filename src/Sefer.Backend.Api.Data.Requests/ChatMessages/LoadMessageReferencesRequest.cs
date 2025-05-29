namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class LoadMessageReferencesRequest : IRequest<bool>
{
    public readonly List<Message> Messages;

    public LoadMessageReferencesRequest(Message message)
    {
        Messages = [message];
    }

    public LoadMessageReferencesRequest(List<Message> messages)
    {
        Messages = messages;
    }
}