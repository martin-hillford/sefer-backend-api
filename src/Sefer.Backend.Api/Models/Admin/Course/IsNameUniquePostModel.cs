// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Course;

/// <summary>
/// This post model is used to check for the uniqueness of the name for a course or series (can be used for both)
/// </summary>
public class IsNameUniquePostModel
{
    /// <summary>
    /// The id (optional) of the course or series being checked.
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// The name to check
    /// </summary>
    public string Name { get; set; }
}
