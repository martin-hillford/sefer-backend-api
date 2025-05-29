// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Admin.Course;

/// <summary>
/// Creates a view on the current editing revision of the course
/// </summary>
/// <inheritdoc />
public class CurrentRevisionView : CourseRevisionView
{
    /// <summary>
    /// A list of all the lessons in this revision
    /// </summary>
    public readonly ReadOnlyCollection<LessonView> Lessons;

    /// <summary>
    /// Creates a view on the revision with lessons
    /// </summary>
    /// <param name="revision">The revision with the lessons</param>
    /// <inheritdoc />
    public CurrentRevisionView(CourseRevision revision) : base(revision)
    {
        var lessons = new List<LessonView>();
        revision?.Lessons?.ToList().ForEach(l => lessons.Add(new LessonView(l)));
        Lessons = lessons.AsReadOnly();
    }
}