namespace Sefer.Backend.Api.Notifications.Mail.Service;

/// <summary>
/// This interface defines the e-mail the service can send.
/// Please note: regarding this interface as internal for Sefer.Backend.Api.Notifications.
/// use the INotificationService to ensure all notifications are send
/// </summary>
public interface IMailService
{
    public Task SendCompleteRegistrationEmailAsync(MailData data);

    public Task SendCompleteInAppRegistrationEmailAsync(MailData data);

    public Task SendPasswordForgotEmailAsync(MailData data);

    public Task SendStudentIsInactiveEmailAsync(MailData data);

    public Task SendPasswordResetCompletedEmailAsync(MailData data);

    public Task SendAccountDeleteConfirmationEmailAsync(MailData data);

    public Task SendEmailUpdateRequestedToOldAddressAsync(MailData data);

    public Task SendEmailUpdateRequestedToNewAddressAsync(MailData data, string newMail);

    public Task SendTwoFactorAuthEnabledEmailAsync(MailData data);

    public Task SendTwoFactorAuthDisabledEmailAsync(MailData data);

    public Task SendEmailUpdateCompleteToOldAddressAsync(MailData data, string newMail, string oldMail);

    public Task SendEmailUpdateCompleteToNewAddressAsync(MailData data, string newMail, string oldMail);

    public Task SendRewardReceivedEmailAsync(MailData data, List<RewardGrant> rewards);

    public Task SendVoucherRewardEmailAsync(MailData data, RewardGrant reward);

    public string SendTestEmail(ISite site);

    public Task SendNotificationEmailAsync(MailData data, List<Message> messages, List<LessonSubmission> submissions);

    public Task SendEnrollmentEmailAsync(MailData data, User enrolledStudent, Course course);
}