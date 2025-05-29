// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Course;

/// <summary>
/// This post model is used to check for the uniqueness of the permalink for a course
/// </summary>
public class IsPermalinkUniquePostModel
{
    /// <summary>
    /// The id (optional) of the course being checked.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The permalink to check
    /// </summary>
    public string Permalink { get; set; }
}
