using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Models.Courses.Surveys;

namespace Sefer.Backend.Api.Views.Shared.Courses.Surveys;

/// <summary>
/// A base view for a course
/// </summary>
/// <inheritdoc />
public class SurveyView : AbstractView<Survey>
{
    #region Properties

    /// <summary>
    /// The CourseRevision this Survey belong to.
    /// </summary>
    public int CourseRevisionId => Model.CourseRevisionId;

    /// <summary>
    ///  When set to true, the user will be offered an survey after completing the course. Default is true.
    /// </summary>
    public bool EnableSurvey => Model.EnableSurvey;

    /// <summary>
    /// When set to true, the user will be asked to rate the course. Default is true.
    /// </summary>
    public bool EnableCourseRating => Model.EnableCourseRating;

    /// <summary>
    /// When set to true, the user will be asked to rate the mentor. Default is true.
    /// </summary>
    public bool EnableMentorRating => Model.EnableMentorRating;

    /// <summary>
    /// When set to true, the user will be given the option to give a testimony (free text). Default is true.
    /// </summary>
    public bool EnableTestimonial => Model.EnableTestimonial;

    /// <summary>
    ///  When set to true, the user will be asked if he allows to post snippets from his testimony online. Default is true.
    /// </summary>
    public bool EnableSocialPermissions => Model.EnableSocialPermissions;

    /// <summary>
    ///  The survey that is a predecessor of this course revision. (Set when a previous survey was promoted)
    /// </summary>
    public int? PredecessorId => Model.PredecessorId;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public SurveyView(Survey model) : base(model) { }

    #endregion
}