// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Models.Admin.Course;

/// <summary>
/// A model for posting the mentors of course
/// </summary>
public class CourseMentorsPostModel
{
    /// <summary>
    /// This list contains all the assigned mentor to the course
    /// </summary>
    public HashSet<int> Mentors { get; set; }
}