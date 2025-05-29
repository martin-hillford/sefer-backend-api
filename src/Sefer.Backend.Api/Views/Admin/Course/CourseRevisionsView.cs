using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Admin.Course;

/// <summary>
/// A view on the course with an editing revision and a published revision
/// </summary>
public class CourseRevisionsView : CourseView
{
    /// <summary>
    /// The current published revision
    /// </summary>
    public readonly CourseRevisionView PublishedRevision;

    /// <summary>
    /// The editing revision of the course
    /// </summary>
    public readonly CurrentRevisionView EditingRevision;

    /// <summary>
    /// Creates a CourseRevisionsView, a view of a course with the editing revision (and lessons) and the published revision
    /// </summary>
    /// <param name="service">A FileService (to generate urls)</param>
    /// <param name="course">The course</param>

    public CourseRevisionsView(IFileStorageService service, Data.Models.Courses.Course course) : base(course, service)
    {
        if (course.PublishedCourseRevision != null) PublishedRevision = new CourseRevisionView(course.PublishedCourseRevision);
        if (course.EditingCourseRevision != null) EditingRevision = new CurrentRevisionView(course.EditingCourseRevision);
    }
}