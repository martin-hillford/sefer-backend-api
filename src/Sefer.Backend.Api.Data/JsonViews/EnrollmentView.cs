namespace Sefer.Backend.Api.Data.JsonViews;

/// <summary>
/// A simple view to show an enrollment in a message, basically it's all about the course
/// </summary>
public class EnrollmentView
{
    /// <summary>
    /// Get the course name for the enrolled course
    /// </summary>
    public string CourseName { get; init; }

    /// <summary>
    /// Create a new view on the enrollment within a chat message
    /// </summary>
    /// <param name="model"></param>
    public EnrollmentView(Enrollment model)
    {
        CourseName = model?.CourseRevision?.Course?.Name;
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    public EnrollmentView() { }
}