// ReSharper disable PropertyCanBeMadeInitOnly.Global, UnusedAutoPropertyAccessor.Global
namespace Sefer.Backend.Api.Data.Models.Courses.Surveys;

/// <summary>
/// Class Survey deals with the survey for a course(revision) which is being presented to the user upon completion of
/// a course. Survey are linked with CourseRevision and modified accordingly within an editable CourseRevision
/// </summary>
/// <inheritdoc cref="ModifyDateLogEntity"/>
public class Survey : ModifyDateLogEntity
{
    #region Properties

    /// <summary>
    /// The CourseRevision this Survey belong to.
    /// </summary>
    [Required]
    public int CourseRevisionId { get; set; }

    /// <summary>
    ///  When set to true, the user will be offered a survey after completing the course. Default is true.
    /// </summary>
    [Required]
    public bool EnableSurvey { get; set; }

    /// <summary>
    /// When set to true, the user will be asked to rate the course. Default is true.
    /// </summary>
    [Required]
    public bool EnableCourseRating { get; set; }

    /// <summary>
    /// When set to true, the user will be asked to rate the mentor. Default is true.
    /// </summary>
    [Required]
    public bool EnableMentorRating { get; set; }

    /// <summary>
    /// When set to true, the user will be given the option to give a testimony (free text). Default is true.
    /// </summary>
    [Required]
    public bool EnableTestimonial { get; set; }

    /// <summary>
    ///  When set to true, the user will be asked if he allows to post snippets from his testimony online. Default is true.
    /// </summary>
    [Required]
    public bool EnableSocialPermissions { get; set; }

    /// <summary>
    ///  The survey that is a predecessor of this course revision. (Set when a previous survey was promoted)
    /// </summary>
    public int? PredecessorId { get; set; }

    /// <summary>
    /// The CourseRevision this Survey belong to.
    /// </summary>
    [ForeignKey("CourseRevisionId")]
    public CourseRevision CourseRevision { get; set; }

    /// <summary>
    ///  The  survey that is a predecessor of this course revision. (Set when a previous survey was promoted)
    /// </summary>
    [ForeignKey("PredecessorId")]
    public Survey Predecessor { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new survey (some default values have been set)
    /// </summary>
    public Survey()
    {
        EnableSurvey = true;
        EnableCourseRating = true;
        EnableMentorRating = true;
        EnableSocialPermissions = true;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a successor survey for the given revision
    /// </summary>
    /// <param name="newRevision">The revision for which the new survey is created</param>
    /// <returns>Created successor</returns>
    public Survey CreateSuccessor(CourseRevision newRevision)
    {
        var successor = new Survey
        {
            CourseRevisionId = newRevision.Id,
            CreationDate = DateTime.UtcNow,
            EnableCourseRating = EnableCourseRating,
            EnableMentorRating = EnableMentorRating,
            EnableSocialPermissions = EnableSocialPermissions,
            EnableSurvey = EnableSurvey,
            EnableTestimonial = EnableTestimonial,
            PredecessorId = Id,
        };
        return successor;
    }

    #endregion
}