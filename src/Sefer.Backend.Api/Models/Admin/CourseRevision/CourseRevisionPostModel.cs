// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.CourseRevision;

/// <summary>
/// A post model for patching a course revision
/// </summary>
public class CourseRevisionPostModel
{
    /// <summary>
    /// When this is set to true, Students are allowed to take this course
    /// without the aid of a mentor
    /// </summary>
    [Required]
    public bool AllowSelfStudy { get; set; }
    
    /// <summary>
    /// Contains general course information that can be displayed with each lesson
    /// </summary>
    public string GeneralInformation { get; set; }
}
