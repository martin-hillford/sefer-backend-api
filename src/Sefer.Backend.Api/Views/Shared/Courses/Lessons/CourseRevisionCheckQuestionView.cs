// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global 
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Shared.Courses.Lessons;

/// <summary>
/// This view is be used for admins, mentors and supervisors to provide a full overview of the questions in a course
/// </summary>
public class CourseRevisionCheckQuestionView : CourseRevisionView
{
    /// <summary>
    /// The course revision belongs to
    /// </summary>
    public readonly CourseView Course;

    /// <summary>
    /// The lessons with all their questions
    /// </summary>
    public readonly List<LessonCheckQuestionView> Lessons;

    /// <summary>
    /// Create a new view with the lessons (holding the questions )
    /// </summary>
    /// <param name="courseRevision">The course revision for which the lessons and question are loaded</param>
    /// <param name="lessons">A list with all the lessons </param>
    /// <param name="course">The course the revision belongs to</param>
    /// <returns></returns>
    public CourseRevisionCheckQuestionView(Course course, List<LessonCheckQuestionView> lessons, CourseRevision courseRevision) : base(courseRevision)
    {
        Lessons = lessons ?? new List<LessonCheckQuestionView>();
        Course = new CourseView(course);
    }
}