// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Data.Models.Courses.Curricula;

namespace Sefer.Backend.Api.Views.Shared.Courses.Curricula;

/// <summary>
/// A basic view on a curriculum revision
/// </summary>
/// <inheritdoc />
public class CurriculumRevisionView : AbstractView<CurriculumRevision>
{
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
    /// Get the number of years in this curriculum
    /// </summary>
    public byte Years => Model.Years;

    /// <summary>
    /// Gets if years are used within the curriculum
    /// </summary>
    public bool UseYears => Model.Years != 0;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public CurriculumRevisionView(CurriculumRevision model) : base(model) { }
}
