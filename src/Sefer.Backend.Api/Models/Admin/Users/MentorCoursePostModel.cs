// ReSharper disable CollectionNeverUpdated.Global
namespace Sefer.Backend.Api.Models.Admin.Users;

/// <summary>
/// This is post model used for posting the courses of a mentor
/// </summary>
public class MentorCoursePostModel
{
    /// <summary>
    /// The list of course id that are mentored by the mentor
    /// </summary>
    public List<int> Courses { get; set; } = [];
}