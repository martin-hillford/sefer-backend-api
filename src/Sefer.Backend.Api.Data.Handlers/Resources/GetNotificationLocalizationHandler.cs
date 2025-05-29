namespace Sefer.Backend.Api.Data.Handlers.Resources;

public class GetNotificationLocalizationHandler(IServiceProvider serviceProvider)
    : Handler<GetNotificationLocalizationRequest, NotificationLocalization>(serviceProvider)
{
    public override async Task<NotificationLocalization> Handle(GetNotificationLocalizationRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.NotificationLocalizations
            .SingleOrDefaultAsync(t => t.Name == request.Name && t.Language == request.Language, token);
    }
}