// This is a view, so properties may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Shared.Courses;

/// <summary>
/// A simple view on a CourseRevision
/// </summary>
/// <inheritdoc />
public class CourseRevisionView : AbstractView<CourseRevision>
{
    #region Properties

    /// <summary>
    /// The stage of the revision
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Stages Stage => Model.Stage;

    /// <summary>
    /// The version of the revision (increasing number)
    /// </summary>
    public int Version => Model.Version;

    /// <summary>
    /// Get or set the id of the predecessor
    /// </summary>
    public int? PredecessorId => Model.PredecessorId;

    /// <summary>
    /// When this is set to true, Students are allowed to take this course
    /// without the aid of a mentor
    /// </summary>
    public bool AllowSelfStudy => Model.AllowSelfStudy;

    /// <summary>
    /// Contains general course information that can be displayed with each lesson
    /// </summary>
    public string GeneralInformation => Model.GeneralInformation;

    /// <summary>
    /// Gets the CourseId of this revision
    /// </summary>
    public int CourseId => Model.CourseId;

    /// <summary>
    /// Gets or set the id of the survey for this lesson
    /// </summary>
    public int? SurveyId => Model.CourseId;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public CourseRevisionView(CourseRevision model) : base(model) { }

    #endregion
}
