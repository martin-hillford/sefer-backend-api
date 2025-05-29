namespace Sefer.Backend.Api.Data.Handlers.ChatMessages;

public class LoadMessageReferencesHandler(IServiceProvider serviceProvider)
    : Handler<LoadMessageReferencesRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(LoadMessageReferencesRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        foreach (var message in request.Messages)
        {
            message.Channel = await context.ChatChannels
                .AsNoTracking()
                .SingleAsync(c => c.Id == message.ChannelId, token);

            message.ChannelMessages = await context.ChatChannelMessages
                .AsNoTracking()
                .Where(c => c.MessageId == message.Id)
                .ToListAsync(token);

            message.Sender = await context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == message.SenderId, token);
        }
        return true;
    }
}