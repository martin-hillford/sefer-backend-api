namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetUsersForNotificationHandler(IServiceProvider serviceProvider)
    : Handler<GetUsersForNotificationRequest, List<User>>(serviceProvider)
{
    public override async Task<List<User>> Handle(GetUsersForNotificationRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.ChatChannelMessages
            .AsNoTracking()
            .Where
                (
                    c =>
                        c.IsNotified == false &&
                        c.ReadDate == null &&
                        c.Message.Type != MessageTypes.StudentEnrollment &&
                        c.Receiver.NotificationPreference == request.MailPreference &&
                        c.Receiver.Blocked == false &&
                        c.Receiver.Approved
                )
            .Select(c => c.Receiver)
            .Distinct()
            .ToListAsync(token);
    }
}