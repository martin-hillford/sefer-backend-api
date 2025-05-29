// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Public.Users;

/// <summary>
/// A model for posting the enrollment into a course
/// </summary>
public class EnrollmentPostModel
{
    /// <summary>
    /// The id of the course to enroll into.
    /// </summary>
    /// <remarks>UserId comes from the logon information</remarks>
    public int CourseId { get; set; }
}
