namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class GetChatMessagesRequest(int channelId, int skip, int take) : IRequest<List<Message>>
{
    public readonly int ChannelId = channelId;

    public readonly int Take = take;

    public readonly int Skip = skip;
}