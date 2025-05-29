namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public abstract class PostChatMessageHandler<TRequest, TResponse>(IServiceProvider serviceProvider)
    : Handler<TRequest, TResponse>(serviceProvider)
    where TRequest : IRequest<TResponse>
{
    protected Task<Message> Post(string text, int sender, int channel, MessageTypes type, CancellationToken token)
        => Post(text, null, string.Empty, sender, channel, type, token);

    protected async Task<Message> Post(string text, int? quotedId, string quotedText, int sender, int channel, MessageTypes type, CancellationToken token)
    {
        try
        {
            // Check if the user in channel
            var isInChannel = await Send(new IsUserInChannelRequest(channel, sender), token);
            if (!isInChannel) return null;

            // If this is quoted message, let's check if that message is in the channel
            var isValidQuote = await IsValidQuote(quotedId, quotedText, channel, token);
            if (!isValidQuote) return null;

            // Insert the message
            var message = new Message
            {
                ChannelId = channel,
                SenderId = sender,
                SenderDate = DateTime.UtcNow,
                Type = type,
                ContentString = text,
                QuotedMessageId = quotedId,
                QuotedMessageString = quotedText,
            };

            // Check if all the data is valid
            if (!await IsValidAsync(message)) return null;

            // Insert the chat message itself
            await using var context = GetDataContext();
            context.ChatMessages.Add(message);
            await context.SaveChangesAsync(token);

            // Insert all the metadata for each user in the channel
            var receivers = context.ChatChannelReceivers.Where(r => r.ChannelId == channel).ToList();
            foreach (var receiver in receivers)
            {
                var meta = new ChannelMessage { MessageId = message.Id, ReceiverId = receiver.UserId };
                if (receiver.UserId == sender) meta.ReadDate = DateTime.UtcNow;
                context.ChatChannelMessages.Add(meta);
            }

            // The Message is inserted, the content added and the receivers send, save it and done.
            await context.SaveChangesAsync(token);

            message.IsAvailable = true;
            context.ChatMessages.Update(message);
            await context.SaveChangesAsync(token);

            // Ensure the channel is loaded
            context.Reference(message, m => m.Channel);
            message.ChannelMessages = context.ChatChannelMessages.Where(m => m.MessageId == m.Id).Include(r => r.Receiver).ToList();

            // Ensure for quoted messages that that message and it's sender are loaded
            if (quotedId == null) return message;

            context.Reference(message, m => m.QuotedMessage);
            context.Reference(message.QuotedMessage, m => m.Sender);

            return message;
        }
        catch (Exception) { return null; }
    }

    private async Task<bool> IsValidQuote(int? quotedId, string quotedText, int channel, CancellationToken token)
    {
        if (quotedId == null) return true;
        if (string.IsNullOrEmpty(quotedText)) return false;

        await using var context = GetDataContext();
        var quotedMessage = await context.ChatMessages.AsNoTracking().SingleOrDefaultAsync(m => m.Id == quotedId.Value, token);
        return quotedMessage != null && quotedMessage.ChannelId == channel;
    }

    protected async Task<object> CreateLessonSubmissionView(int submissionId, CancellationToken token)
    {
        var submission = await Send(new GetFinalSubmissionByIdRequest(submissionId), token);
        if (submission == null) return null;

        if (submission.ResultsStudentVisible) return new ReviewedSubmissionView(submission);
        return new SubmissionView(submission);
    }
}