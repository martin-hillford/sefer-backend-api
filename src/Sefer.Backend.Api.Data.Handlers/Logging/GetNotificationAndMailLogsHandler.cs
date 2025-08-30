namespace Sefer.Backend.Api.Data.Handlers.Logging;

public class GetNotificationAndMailLogsHandler(IServiceProvider serviceProvider)
    : Handler<GetNotificationAndMailLogsRequest, List<Log>>(serviceProvider)
{
    public override async Task<List<Log>> Handle(GetNotificationAndMailLogsRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Logs
            .AsNoTracking()
            .Where(l => l.LogLevel != "Debug")
            .Where
                (
                    l =>
                        l.CategoryName == "Sefer.Backend.Api.Notifications.Mail.NotificationServiceProcessor" ||
                        l.CategoryName == "Sefer.Backend.Api.Services.Mail.MailServiceBase" ||
                        l.CategoryName == "Sefer.Backend.Api.Notifications.Mail.Service.MailService" ||
                        l.CategoryName == "Sefer.Backend.Api.Notifications.Mail.EmailDigestService" ||
                        l.CategoryName == "Sefer.Backend.Api.Notifications.NotificationService" ||
                        l.CategoryName == "Sefer.Backend.Api.Notifications.Push.FireBase"
                )
            .OrderByDescending(log => log.Timestamp)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(token);
    }
}

