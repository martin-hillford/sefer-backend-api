using Sefer.Backend.Api.Data.Requests.Resources;

namespace Sefer.Backend.Api.Notifications.Push;

public static class Localization
{
    private static async Task<string> Get(IMediator mediator, string language, string key)
    {
        var request = new GetNotificationLocalizationRequest(key, language);
        var localization = await mediator.Send(request);
        if (localization == null) throw new Exception($"Notification localization with name {key} not found");
        return localization.Content;
    }

    public static async Task<(string Title, string Body)> GetContent(IMediator mediator, string language, string key)
    {
        return
        (
            await Get(mediator, language, key + "Title"),
            await Get(mediator, language, key + "Body")
        );
    }
}

