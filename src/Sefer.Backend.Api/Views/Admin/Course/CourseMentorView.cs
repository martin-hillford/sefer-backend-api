// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;
using DataCourse = Sefer.Backend.Api.Data.Models.Courses.Course;
using DataMentor = Sefer.Backend.Api.Data.Models.Users.User;

namespace Sefer.Backend.Api.Views.Admin.Course;

/// <summary>
/// CourseMentorView is a view that contains data about the mentors assigned to a course
/// </summary>
public class CourseMentorView : Data.JsonViews.CourseView
{
    /// <summary>
    /// A collection of mentors that is assigned to the course
    /// </summary>
    public IEnumerable<PrimitiveUserView> Assigned  { get; private set; }

    /// <summary>
    /// A collection of mentors that is available to the course
    /// </summary>
    public IEnumerable<PrimitiveUserView> Available  { get; private set; }

    /// <summary>
    /// Creates a view that for a course will also contain
    /// </summary>
    /// <param name="course"></param>
    /// <param name="assigned"></param>
    /// <param name="available"></param>
    /// <returns></returns>
    public CourseMentorView(DataCourse course, List<DataMentor> assigned, List<DataMentor> available) : base(course)
    {
        Assigned = assigned.Select(m => new PrimitiveUserView(m));
        Available = available.Select(m => new PrimitiveUserView(m));
    }
}