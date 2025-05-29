// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Public.Courses;

/// <summary>
/// A series with its courses
/// </summary>
/// <inheritdoc />
public sealed class SeriesListView : SeriesView
{
    /// <summary>
    /// A list of all the courses that are in this series (public available)
    /// </summary>
    public List<int> Courses { get; set; }

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public SeriesListView(Series model) : base(model)
    {
        Courses = model.SeriesCourses.OrderBy(s => s.SequenceId).Select(c => c.CourseId).ToList();
    }
}
