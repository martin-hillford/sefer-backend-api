// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Resources;

/// <summary>
/// The Testimony of users of courses on the website
/// </summary>
/// <inheritdoc />
public class Testimony : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// The (optional) id of the course of which this testimony is about
    /// </summary>
    public int? CourseId { get; set; }

    /// <summary>
    /// The optional id of the student that the testimony.
    /// </summary>
    /// <remarks>Please note, this should not be public!</remarks>
    public int? StudentId { get; set; }

    /// <summary>
    /// Holds a reference to the survey result that is connected with this testimony
    /// </summary>
    /// <value></value>
    public int? SurveyResultId { get; set; }

    /// <summary>
    /// The testimony itself
    /// </summary>
    [Required, MinLength(1), MaxLength(int.MaxValue)]
    public string Content { get; set; }

    /// <summary>
    /// The (optional) name of the person who made this testimony
    /// </summary>
    [MaxLength(255)]
    public string Name { get; set; }

    #endregion

    #region Extended Properties

    /// <summary>
    /// True when the testimony was anonymous
    /// </summary>
    public bool IsAnonymous { get; set; }

    #endregion
}