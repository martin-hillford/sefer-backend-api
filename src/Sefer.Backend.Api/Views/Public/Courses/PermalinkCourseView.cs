// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
// ReSharper disable ClassNeverInstantiated.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Public.Courses;

/// <summary>
/// A simple view for the course, but with a proper permalink
/// </summary>
public class PermalinkCourseView : CourseView
{
    /// <summary>
    /// Gets the permalink for the course. This should be a unique entry.
    /// </summary>
    public new readonly string Permalink;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <inheritdoc />
    public PermalinkCourseView(Course model) : base(model)
    {
        if (string.IsNullOrEmpty(model.Permalink) == false) Permalink = "/course/" + model.Permalink;
        else Permalink = "/public/course/" + model.Id;
    }
}