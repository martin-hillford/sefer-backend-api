// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Views.Shared.Courses;
using DataCourse = Sefer.Backend.Api.Data.Models.Courses.Course;

namespace Sefer.Backend.Api.Views.Admin.Course;

/// <summary>
/// A view on a series together with its available and included courses
/// </summary>
/// <inheritdoc />
public class SeriesCoursesView : SeriesView
{
    /// <summary>
    /// The list of courses that are in the series
    /// </summary>
    public readonly IEnumerable<Data.JsonViews.CourseView> IncludedCourses;

    /// <summary>
    /// The list of available courses to add to the series
    /// </summary>
    public readonly IEnumerable<Data.JsonViews.CourseView> AvailableCourses;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="included">The list of the included courses</param>
    /// <param name="available">The list of available courses</param>
    /// <inheritdoc />
    public SeriesCoursesView(Series model, IEnumerable<DataCourse> included, IEnumerable<DataCourse> available) : base(model)
    {
        IncludedCourses = included.Select(c => new Data.JsonViews.CourseView(c));
        AvailableCourses = available.Select(c => new Data.JsonViews.CourseView(c));
    }
}
