using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Models.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Shared.Courses.Curricula;

/// <summary>
/// A basic view on the curriculum
/// </summary>
/// <inheritdoc cref="AbstractView{TDataContract}"/>
public class CurriculumView : AbstractView<Curriculum>
{
    #region Properties

    /// <summary>
    /// Gets / sets the name of the curriculum
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Gets the permalink for the curriculum. This should be a unique entry.
    /// </summary>
    public string Permalink => Model.Permalink;

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    public string Description => Model.Description;

    /// <summary>
    /// The difficulty level of the curriculum
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level => Model.Level;

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
