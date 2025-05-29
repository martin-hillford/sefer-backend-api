// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.CourseRevision;

/// <summary>
/// A post model for patching a course revision
/// </summary>
public class CourseRevisionPostModel
{
    /// <summary>
    /// When this is set to true; Student are allowed to take this course
    /// without the aid of a mentor
    /// </summary>
    [Required]
    public bool AllowSelfStudy { get; set; }
}
