// This is a view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.Users;

/// <summary>
/// A view on mentors with courses
/// </summary>
/// <inheritdoc />
public class MentorWithCoursesView : UserView
{
    #region Properties

    /// <summary>
    /// The list of courses that is mentor is mentoring
    /// </summary>
    public readonly ReadOnlyCollection<CourseView> Courses;

    /// <summary>
    /// The list of courses that are available for mentoring
    /// </summary>
    public readonly ReadOnlyCollection<CourseView> AvailableCourses;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="courses">The list of courses that is mentor is mentoring</param>
    /// <param name="availableCourses">The list of courses that are available for mentoring</param>
    /// <inheritdoc />
    public MentorWithCoursesView(User model, List<Data.Models.Courses.Course> courses, List<Data.Models.Courses.Course> availableCourses) : base(model)
    {
        var coursesList = new List<CourseView>();
        courses?.ForEach(c => coursesList.Add(new CourseView(c)));
        Courses = coursesList.AsReadOnly();

        var availableCoursesList = new List<CourseView>();
        availableCourses?.ForEach(c => availableCoursesList.Add(new CourseView(c)));
        AvailableCourses = availableCoursesList.AsReadOnly();
    }

    #endregion
}