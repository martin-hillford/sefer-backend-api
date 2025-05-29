// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Enrollments;

/// <summary>
/// This model is used to change an enrollment of a student in a given course
/// </summary>
public class ChangeMentorPostModel
{
    /// <summary>
    /// The id of the enrollment
    /// </summary>
    public int EnrollmentId { get; set; }

    /// <summary>
    /// The id of the new mentor of the student in the enrollment
    /// </summary>
    public int MentorId { get; set; }
}