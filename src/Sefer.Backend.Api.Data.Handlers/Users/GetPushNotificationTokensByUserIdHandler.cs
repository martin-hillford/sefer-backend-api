namespace Sefer.Backend.Api.Data.Handlers.Users;

public class GetPushNotificationTokensByUserIdHandler(IServiceProvider serviceProvider)
    : Handler<GetPushNotificationTokensByUserIdRequest, List<string>>(serviceProvider)
{
    public override async Task<List<string>> Handle(GetPushNotificationTokensByUserIdRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.PushNotificationTokens
            .Where(t => t.UserId == request.UserId)
            .Select(t => t.Token)
            .ToListAsync(token);
    }
}


