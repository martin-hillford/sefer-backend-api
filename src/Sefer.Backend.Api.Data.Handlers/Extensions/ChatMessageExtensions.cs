namespace Sefer.Backend.Api.Data.Handlers.Extensions;

internal static class ChatMessageExtensions
{
    internal static (string text, int? quotedMessageId, string quotedMessageText, int channelId, int userId)
        GetParams(this PostTextChatMessageRequest request)
    {
        return
        (
            request.Text,
            request.QuotedMessageId,
            request.QuotedMessageText,
            request.ChannelId,
            request.SenderId
        );
    }
}