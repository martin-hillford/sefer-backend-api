namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class GetMessageByIdHandler(IServiceProvider serviceProvider)
    : Handler<GetMessageByIdRequest, Message>(serviceProvider)
{
    public override async Task<Message> Handle(GetMessageByIdRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.ChatMessages
            .AsNoTracking()
            .Include(c => c.Channel)
            .Where(m => m.Channel.Receivers.Any(r => r.UserId == request.UserId) && m.Id == request.MessageId)
            .FirstOrDefaultAsync(token);
    }
}