using Sefer.Backend.Api.Data.Models.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Admin.Course.Curricula;

/// <summary>
/// A view on the curriculum that includes the stage of the curriculum
/// </summary>
/// <inheritdoc cref="Shared.Courses.Curricula.CurriculumView"/>
public class CurriculumView : Shared.Courses.Curricula.CurriculumView
{
    #region Properties

    /// <summary>
    /// returns the overall stage of this curriculum
    /// Published: the curriculum has a published revision
    /// Closed: the curriculum has an edit revision and one or closed revisions but no published revision
    /// Edit: the curriculum has only an edit revision but no published or closed one.
    /// </summary>
    /// <returns>the stage of which this whole course is in.</returns>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Stages Stage => Model.OverallStage;

    /// <summary>
    /// Checks if the course is editable which is depending on the stage of the CourseRevision
    /// </summary>
    /// <returns>True when the course is editable else false</returns>
    public bool IsEditable => Model.IsEditable;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public CurriculumView(Curriculum model) : base(model) { }

    #endregion
}
