// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using DataCourse = Sefer.Backend.Api.Data.Models.Courses.Course;

namespace Sefer.Backend.Api.Views.Admin.CoursePrerequisites;

/// <summary>
/// A base view for a course
/// </summary>
/// <inheritdoc />
public class CourseView : Data.JsonViews.CourseView
{
    #region Properties

    /// <summary>
    /// The list of required courses before taking the course
    /// </summary>
    public readonly IEnumerable<Data.JsonViews.CourseView> RequiredCourses;

    /// <summary>
    /// The list of available courses to add as required course
    /// </summary>
    public readonly IEnumerable<Data.JsonViews.CourseView> AvailableCourses;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="required">The list of the direct required courses</param>
    /// <param name="available">The list of available courses</param>
    /// <inheritdoc />
    public CourseView(DataCourse model, IEnumerable<DataCourse> required, IEnumerable<DataCourse> available) : base(model)
    {
        RequiredCourses = required.Select(c => new Data.JsonViews.CourseView(c));
        AvailableCourses = available.Select(c => new Data.JsonViews.CourseView(c));
    }

    #endregion
}