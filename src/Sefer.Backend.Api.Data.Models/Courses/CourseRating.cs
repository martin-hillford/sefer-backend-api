// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// This model contains a rating provided by a user on completing a course
/// </summary>
/// <inheritdoc />
public class CourseRating : Entity
{
    /// <summary>
    /// The date the object was created.
    /// </summary>
    [InsertOnly]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The id of the mentor that was rated
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The rating performed
    /// </summary>
    [Range(0, 10)]
    public byte Rating { get; set; }

    /// <summary>
    /// The mentor of the rating
    /// </summary>
    [ForeignKey("CourseId")]
    public Course Course { get; set; }
}