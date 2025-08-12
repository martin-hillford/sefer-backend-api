namespace Sefer.Backend.Api.Services.Notifications;

public interface INotificationService
{
    /// <summary>
    /// Send notifications when a user updates the name in their profile
    /// </summary>
    public Task SendProfileUpdatedNotificationAsync(int userId, string oldName);

    /// <summary>
    /// Sends notification that the student has submitted a lesson for the mentor to review
    /// </summary>
    public Task<bool> SendLessonSubmittedNotificationAsync(int submissionId, User mentor, User student);

    /// <summary>
    /// Sends a notification that a new chat message is sent
    /// </summary>
    public Task SendChatMessageSendNotificationAsync(ChannelMessage message, bool ignoreHasRead = false);

    /// <summary>
    /// Send a notification that a message is read by the given user
    /// </summary>
    public Task SendChatMessageIsReadNotificationAsync(int messageId, User user);

    /// <summary>
    /// Send notifications when a mentor has reviewed a submission
    /// </summary>
    public Task<bool> SendSubmissionReviewedNotificationAsync(LessonSubmission submission);

    /// <summary>
    /// Sends notifications when a student enrolls into a course
    /// </summary>
    public Task SendStudentEnrolledInCourseNotificationAsync(Course course, Enrollment enrollment);

    /// <summary>
    /// Send a notification to the user because he is inactive
    /// </summary>
    public Task SendStudentIsInactiveNotificationAsync(User student);

    /// <summary>
    /// Send notifications to all inactive users
    /// </summary>
    public Task SendStudentIsInactiveNotificationAsync();

    /// <summary>
    /// Send a notification to the user that is just registered.
    /// </summary>
    Task SendCompleteRegistrationNotificationAsync(User student, string language);

    /// <summary>
    /// Send a notification to the user that is just registered.
    /// </summary>
    Task SendCompleteInAppRegistrationNotificationAsync(User student, string language);

    /// <summary>
    /// Sends a notification to a user to send a password forgotten message
    /// </summary>
    Task SendPasswordForgotNotificationAsync(User user, string language);

    /// <summary>
    /// Sends a notification to a user to send a password-reset message
    /// </summary>
    Task SendPasswordResetCompletedNotificationAsync(User user, string language);

    /// <summary>
    /// Sends a notification to the user to update his e-mail
    /// </summary>
    Task SendEmailUpdateRequestedNotificationAsync(User user, string language, string newMail);

    /// <summary>
    /// Sends notification to the user to notify it that his e-mail is updated to the new mail
    /// </summary>
    Task SendEmailUpdateCompleteNotificationAsync(User user, string language, string newMail, string oldMail);

    /// <summary>
    /// Send a notification to a user to ask for confirmation to delete its account
    /// </summary>
    Task SendAccountDeleteConfirmationNotificationAsync(User user, string language);

    /// <summary>
    /// Send notifications about a student getting a reward
    /// </summary>
    Task SendRewardReceivedNotificationAsync(User data, List<RewardGrant> rewards);

    /// <summary>
    /// Send a notification to the user when he has disabled two-factor auth
    /// </summary>
    Task SendTwoFactorAuthDisabledNotificationAsync(User user, string language);

    /// <summary>
    /// Send a notification to the user when he has enabled two-factor auth
    /// </summary>
    Task SendTwoFactorAuthEnabledNotificationAsync(User user, string language);

    /// <summary>
    /// This method is used to send a test notification
    /// </summary>
    string SendTestNotification(ISite site);
}