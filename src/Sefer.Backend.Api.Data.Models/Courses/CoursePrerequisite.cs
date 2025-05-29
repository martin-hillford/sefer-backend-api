// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// Defines a requirement relationship between courses
/// </summary>
/// <inheritdoc cref="Entity"/>
public class CoursePrerequisite : Entity
{
    #region Properties

    /// <summary>
    /// The identifier of the course this prerequisite is all about
    /// </summary>
    [ForeignKey("Course"), Required]
    public int CourseId { get; set; }

    /// <summary>
    /// The identifier of the course that is required to take before the course (with the given courseId) can be followed.
    /// </summary>
    [ForeignKey("RequiredCourse"), Required]
    public int RequiredCourseId { get; set; }

    #endregion

    #region References

    /// <summary>
    /// The course that a has a required course
    /// </summary>
    public Course Course { get; set; }

    /// <summary>
    /// The course that is required
    /// </summary>
    public Course RequiredCourse { get; set; }

    #endregion
}