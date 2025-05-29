// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Student;

/// <summary>
/// Defines the different types of activity for the stream
/// </summary>
public enum ActivityTypes : short
{
    /// <summary>
    /// Indicates the logon of the user
    /// </summary>
    Login,

    /// <summary>
    /// Indicates the enrollment into the course by a student
    /// </summary>
    Enrollment,

    /// <summary>
    /// Indicates the user has submitted a lesson
    /// </summary>
    Submission,

    /// <summary>
    /// Indicate the user has created his profile
    /// </summary>
    ProfileCreation
}

/// <summary>
/// The ActivityView is a view used to display activity of the student.
/// </summary>
public class ActivityView
{
    #region Properties

    /// <summary>
    /// The type of the activity
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public readonly ActivityTypes Type;

    /// <summary>
    /// The time of the activity
    /// </summary>
    public readonly DateTime? Time;

    /// <summary>
    /// (optional) the course (for an enrollment)
    /// </summary>
    public readonly CourseView Course;

    /// <summary>
    /// (optional) the lesson (for a submission)
    /// </summary>
    public readonly Public.Lessons.LessonView Lesson;

    #endregion

    #region Constructors

    /// <summary>
    /// Create an activity from an LessonSubmission
    /// </summary>
    public ActivityView(LessonSubmission submission, IFileStorageService fileStorageService)
    {

        Type = ActivityTypes.Submission;
        Time = submission.SubmissionDate;
        Course = new CourseView(submission.Lesson.CourseRevision.Course);
        Lesson = new Public.Lessons.LessonView(submission.Lesson, fileStorageService);
    }

    /// <summary>
    /// Create an activity from an loginLogEntry
    /// </summary>
    /// <param name="loginLogEntry"></param>
    public ActivityView(LoginLogEntry loginLogEntry)
    {
        Type = ActivityTypes.Login;
        Time = loginLogEntry.LogTime;
    }

    /// <summary>
    /// Create an activity from an enrollment
    /// </summary>
    /// <param name="enrollment"></param>
    public ActivityView(Enrollment enrollment)
    {
        Type = ActivityTypes.Enrollment;
        Time = enrollment.CreationDate;
        Course = new CourseView(enrollment.CourseRevision.Course);
    }

    #endregion
}
