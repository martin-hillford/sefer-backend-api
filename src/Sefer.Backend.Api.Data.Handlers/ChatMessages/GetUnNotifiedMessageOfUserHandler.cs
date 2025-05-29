namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class GetUnNotifiedMessageOfUserHandler(IServiceProvider serviceProvider)
    : Handler<GetUnNotifiedMessageOfUserRequest, List<Message>>(serviceProvider)
{
    public override async Task<List<Message>> Handle(GetUnNotifiedMessageOfUserRequest request, CancellationToken token)
    {
        // These are all the unread message of the given user
        var sendDate = DateTime.UtcNow.AddSeconds(-1 * request.Delay);
        await using var context = GetDataContext();
        return await context.ChatChannelMessages
            .AsNoTracking()
            .Include(m => m.Message).ThenInclude(m => m.Sender)
            .Include(m => m.Message).ThenInclude(m => m.QuotedMessage).ThenInclude(m => m.Sender)
            .Where(m =>
                m.ReceiverId == request.User &&
                m.IsNotified == false &&
                m.Message.Type != MessageTypes.StudentEnrollment &&
                m.ReadDate == null && m.Message.SenderDate < sendDate
            )
            .Select(m => m.Message)
            .ToListAsync(token);
    }
}