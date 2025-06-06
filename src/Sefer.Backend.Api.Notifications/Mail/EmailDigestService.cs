using Sefer.Backend.Api.Data.Requests.Submissions;

namespace Sefer.Backend.Api.Notifications.Mail;

public class EmailDigestService(IServiceProvider serviceProvider) : IEmailDigestService
{
    private readonly IMediator _mediator = serviceProvider.GetService<IMediator>();

    private readonly IMailService _mailService = serviceProvider.GetService<IMailService>();

    private readonly ILogger<EmailDigestService> _logger = serviceProvider.GetService<ILogger<EmailDigestService>>();

    public async Task SendDirectNotificationsDigestAsync()
    {
        // Logging information for debugging notifications
        _logger.LogInformation("Running direct notifications tasks.");

        // Sending the notifications
        await SendNotificationsDigestAsync(NotificationPreference.Direct, 180);
    }

    public async Task SendDailyNotificationsDigestAsync()
    {
        // Logging information for debugging notifications
        _logger.LogInformation("Running daily notifications tasks.");

        // Sending the notifications
        await SendNotificationsDigestAsync(NotificationPreference.DailyDigest);
    }

    public async Task SendWeeklyNotificationsDigestAsync()
    {
        // Logging information for debugging notifications
        _logger.LogInformation("Running daily notifications tasks.");

        // Sending the notifications
        await SendNotificationsDigestAsync(NotificationPreference.WeeklyDigest);
    }

    public async Task SendNotificationsDigestAsync(NotificationPreference notificationPreference, int delay = 0)
    {
        // First step, get all the users who have the given duration set
        var users = await _mediator.Send(new GetUsersForNotificationRequest(notificationPreference));
        if (users.Count == 0) return;

        // Logging information for debugging notifications

        var prefLabel = notificationPreference.GetLabel();
        _logger.LogInformation($"Sending '{prefLabel}' notifications for {users.Count} users.");

        // Now we loop through all the users and send the notification
        var task = new List<Task>();
        users.ForEach(u => task.Add(SendNotificationsDigestAsync(u, delay)));
        await Task.WhenAll(task);
    }

    public Task SendNotificationsDigestAsync(User user) => SendNotificationsDigestAsync(user, 0);

    private async Task SendNotificationsDigestAsync(User user, int delay)
    {
        try
        {
            // Get the user
            if (user == null) return;

            // Get the new messages a user has
            var messages = await _mediator.Send(new GetUnNotifiedMessageOfUserRequest(user.Id, delay));
            var submissions = await _mediator.Send(new GetSubmissionsForReviewRequest(user.Id));

            // Note: when a lessons submission, a messages is also being sent. So only start sending when there are
            // unnotified message
            if (messages.Count == 0) return;

            // Logging information for debugging notifications
            _logger.LogInformation($"Sending notifications for user: {user.Id}, messages: {messages.Count}, submissions: {submissions.Count}");

            // Send the email to the user
            var (region, site) = await _mediator.Send(new GetPrimaryRegionAndSiteRequest(user.Id));
            var data = new MailData(serviceProvider, user, site, region, user.GetPreferredInterfaceLanguage());
            await _mailService.SendNotificationEmailAsync(data, messages, submissions);

            // And now mark them being notified
            var messageIds = messages.Select(m => m.Id).ToList();
            var tasks = messageIds.Select(m => _mediator.Send(new MarkMessageForUserNotifiedRequest(m, user.Id)));
            await Task.WhenAll(tasks);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error occurred while sending notifications to user {user?.Id}");
        }
    }
}