using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Shared.Courses;

/// <summary>
/// A basic view on the series
/// </summary>
/// <inheritdoc cref="AbstractView{TDataContract}"/>
public class SeriesView : AbstractView<Series>
{
    #region Properties

    /// <summary>
    /// Gets / sets the name of the course
    /// </summary>
    public string Name => Model.Name;

    /// <summary>
    /// Gets / sets the description for the course.
    /// </summary>
    public string Description => Model.Description;

    /// <summary>
    /// The difficulty level of the courses
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Levels Level => Model.Level;

    /// <summary>
    /// When this is set to true, the Series will be displayed for users
    /// (students and visitors on the website)
    /// </summary>
    public bool IsPublic => Model.IsPublic;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public SeriesView(Series model) : base(model) { }

    #endregion
}
