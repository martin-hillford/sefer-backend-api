

namespace Sefer.Backend.Api.Notifications;

/// <summary>
/// The email background service is taking care of sending notifications to users
/// </summary>
/// <remarks>
/// Creates a new notification service.
/// </remarks>
public class BackgroundService(IEmailDigestService digestService, INotificationService notificationService) : IHostedService, IDisposable
{
    /// <summary>
    /// A time that will help with deal with daily sending the e-mail
    /// </summary>
    private Timer _dailyTimer;

    /// <summary>
    /// A timer that will help with deal with the weekly e-mail
    /// </summary>
    private Timer _weeklyTimer;

    /// <summary>
    /// A timer that will deal with sending the direct message (1 or 2 minutes delay)
    /// </summary>
    private Timer _directTimer;

    /// <summary>
    /// This method will start the task of sending the notifications
    /// </summary>
    /// <param name="cancellationToken"></param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (EnvVar.GetEnvironmentVariable("DIGEST_SERVICE_DISABLED") == "true") return Task.CompletedTask;
        
        var timeSpan = DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow;
        _dailyTimer = new Timer(SendDailyNotifications, null, timeSpan, TimeSpan.FromDays(1));
        _weeklyTimer = new Timer(SendWeeklyNotifications, null, timeSpan, TimeSpan.FromDays(7));
        _directTimer = new Timer(SendDirectNotifications, null, TimeSpan.Zero, TimeSpan.FromMinutes(2));

        return Task.CompletedTask;
    }

    /// <summary>
    /// This method will stop the task of sending the notifications
    /// </summary>
    /// <param name="cancellationToken"></param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _dailyTimer?.Change(Timeout.Infinite, 0);
        _weeklyTimer?.Change(Timeout.Infinite, 0);
        _directTimer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    /// <summary>
    /// This method will dispose the notification service
    /// </summary>
    public void Dispose()
    {
        _dailyTimer?.Dispose();
        _weeklyTimer?.Dispose();
        _directTimer?.Dispose();
    }

    /// <summary>
    /// Send daily notifications
    /// </summary>
    private void SendDailyNotifications(object _)
    {
        Task.Run(SendDailyNotificationsAsync);
    }

    /// <summary>
    /// Send daily notifications
    /// </summary>
    private async Task SendDailyNotificationsAsync()
    {
        // We need to deal daily message notifications, but also the activity notification
        await digestService.SendDailyNotificationsDigestAsync();
        await notificationService.SendStudentIsInactiveNotificationAsync();
    }

    /// <summary>
    /// Send weekly notifications
    /// </summary>
    private void SendWeeklyNotifications(object _)
    {
        Task.Run(SendWeeklyNotificationsAsync);
    }

    /// <summary>
    /// Send weekly notifications
    /// </summary>
    private async Task SendWeeklyNotificationsAsync()
    {
        await digestService.SendWeeklyNotificationsDigestAsync();
    }

    /// <summary>
    /// Send weekly notifications
    /// </summary>
    private void SendDirectNotifications(object _)
    {
        Task.Run(digestService.SendDirectNotificationsDigestAsync);
    }
}