// This is post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
// ReSharper disable MemberCanBeProtected.Global UnusedAutoPropertyAccessor.Global

namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// Model for a sending notifications
/// </summary>
public class NotificationModel : MailModel
{
    /// <summary>
    /// The user that was registered
    /// </summary>
    public readonly User User;

    /// <summary>
    /// The number of submissions
    /// </summary>
    public List<LessonSubmission> Submissions { get; init; }

    /// <summary>
    /// The number of unread messages
    /// </summary>
    public List<NotificationMessage> Messages { get; init; }

    /// <summary>
    /// Creates a new PasswordReset
    /// </summary>
    public NotificationModel(MailData mailData) : base(mailData)
    {
        User = mailData.Receiver;
    }
}