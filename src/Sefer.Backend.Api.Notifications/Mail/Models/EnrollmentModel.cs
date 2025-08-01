// This is the post model to an external service, so not all properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Notifications.Mail.Models;

/// <summary>
/// Model for a sending an email about the enrollment of a student into a course
/// </summary>
public class EnrollmentModel : MailModel
{
    /// <summary>
    /// The course that the student enrolled in
    /// </summary>
    public string CourseName { get; init; }

    /// <summary>
    /// The mentor supervising the course
    /// </summary>
    public string MentorName { get; init; }

    /// <summary>
    /// The student taking the course
    /// </summary>
    public string StudentName { get; init; }

    /// <summary>
    /// Creates a new PasswordReset
    /// </summary>
    public EnrollmentModel(MailData mailData) : base(mailData) { }
}