
namespace Sefer.Backend.Api.Data.Handlers.Users;

public class AddPushNotificationTokenHandler(IServiceProvider serviceProvider)
    : Handler<AddPushNotificationTokenRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(AddPushNotificationTokenRequest request, CancellationToken token)
    {
        var user = await Send(new GetUserByIdRequest(request.UserId), token);
        if (user == null) return false;

        var context = GetDataContext();
        var exists = await context.PushNotificationTokens.SingleOrDefaultAsync(p => p.Token == request.Token, token);

        try
        {
            if (exists != null && exists.UserId != request.UserId)
            {
                const string update = "UPDATE push_notification_tokens SET user_id = {0} WHERE token = {1}";
                await context.Database.ExecuteSqlRawAsync(update, request.UserId, request.Token);
            }
            else if (exists == null)
            {
                const string insert = "INSERT INTO push_notification_tokens (token, user_id) VALUES({0}, {1})";
                await context.Database.ExecuteSqlRawAsync(insert, request.Token, request.UserId);
            }
            return true;
        }
        catch (Exception) { return false; }
    }
}