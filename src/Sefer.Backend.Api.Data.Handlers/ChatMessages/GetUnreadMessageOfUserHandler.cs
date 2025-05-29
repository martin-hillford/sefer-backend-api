namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class GetUnreadMessageOfUserHandler(IServiceProvider serviceProvider)
    : Handler<GetUnreadMessageOfUserRequest, List<Message>>(serviceProvider)
{
    public override async Task<List<Message>> Handle(GetUnreadMessageOfUserRequest request, CancellationToken token)
    {
        // These are all the unread message of the given user
        await using var context = GetDataContext();
        return await context.ChatChannelMessages
            .AsNoTracking()
            .Where(m => m.ReceiverId == request.User && m.ReadDate == null && m.Message.SenderId != request.User && m.Message.SenderDate < DateTime.UtcNow)
            .Include(m => m.Message).ThenInclude(m => m.Sender)
            .Include(m => m.Message).ThenInclude(m => m.QuotedMessage).ThenInclude(m => m.Sender)
            .Select(m => m.Message)
            .ToListAsync(token);
    }
}