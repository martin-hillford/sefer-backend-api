namespace Sefer.Backend.Api.Data.Models.Constants;

/// <summary>
/// Users can select their preferred way for notifications
/// </summary>
public enum NotificationPreference : byte { Direct = 1, DailyDigest = 2, WeeklyDigest = 3, None = 4 }

public static class Extensions
{
    public static string GetLabel(this NotificationPreference mailNotificationPreference)
    {
        return mailNotificationPreference switch
        {
            NotificationPreference.Direct => "Direct",
            NotificationPreference.DailyDigest => "DailyDigest",
            NotificationPreference.WeeklyDigest => "WeeklyDigest",
            _ => "None"
        };
    }
}
