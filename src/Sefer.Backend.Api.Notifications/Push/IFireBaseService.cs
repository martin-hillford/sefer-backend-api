namespace Sefer.Backend.Api.Notifications.Push;

/// <summary>
/// This interface defines the notifications the push notification service can send.
/// Please note: regarding this interface as internal for Sefer.Backend.Api.Notifications.
/// use the INotificationService to ensure all notifications (including) are send
/// </summary>
public interface IFireBaseService
{
    Task SendLessonSubmittedNotificationToMentor(int submissionId, User mentor, User student);

    Task SendLessonReviewedNotificationToStudent(int submissionId, User student);

    Task<bool> SendChatTextMessageNotification(int userId, string title, string body, bool throwExceptions = false);

    Task SendStudentIsInactiveNotificationAsync(User student);

    Task SendStudentEnrolledNotificationToMentor(Course course, Enrollment enrollment);
}
