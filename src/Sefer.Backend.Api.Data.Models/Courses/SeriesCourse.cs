// This is an entity framework model so some properties may not be set in code
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace Sefer.Backend.Api.Data.Models.Courses;

/// <summary>
/// The SeriesCourse class defines that a course is part of series
/// </summary>
/// <inheritdoc cref="Entity"/>
public class SeriesCourse : Entity
{
    #region Properties

    /// <summary>
    /// The identifier of the course that is within the series
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// The identifier of the series that the course is in
    /// </summary>
    public int SeriesId { get; set; }

    /// <summary>
    /// The sort order of the course within the series
    /// </summary>
    public int SequenceId { get; set; }

    #endregion

    #region References

    /// <summary>
    /// The course that is in the series
    /// </summary>
    public Course Course { get; set; }

    /// <summary>
    /// The series that contains the course
    /// </summary>
    public Series Series { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Empty constructor
    /// </summary>
    public SeriesCourse() { }

    /// <summary>
    /// Creates a new relation between the series and the course
    /// </summary>
    /// <param name="seriesId">The id of the series</param>
    /// <param name="courseId">The id of the course</param>
    /// <param name="sequenceId">The sequence id</param>
    public SeriesCourse(int seriesId, int courseId,int sequenceId)
    {
        CourseId = courseId;
        SeriesId = seriesId;
        SequenceId = sequenceId;
    }

    #endregion
}