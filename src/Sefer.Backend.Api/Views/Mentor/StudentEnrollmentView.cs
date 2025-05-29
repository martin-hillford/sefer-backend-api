// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// The view of the mentor on the student
/// </summary>
public class StudentEnrollmentView : AbstractView<Enrollment>
{
    /// <summary>
    /// Gets the name of the user
    /// </summary>
    public string Name => Model.Student.Name;

    /// <summary>
    /// Gets the id of the student (useful for going to a detail view)
    /// </summary>
    public int StudentId => Model.StudentId;

    /// <summary>
    /// Returns the date the student enrolled in his last course
    /// </summary>
    public DateTime CourseEnrollmentDate => Model.CreationDate;

    /// <summary>
    /// Returns if the student has completed that course
    /// </summary>
    public bool CourseCompleted => Model.IsCourseCompleted;

    /// <summary>
    /// The name of the course of student is/was enrolled in
    /// </summary>
    public string CourseName => Model.CourseRevision.Course.Name;

    /// <summary>
    /// Full view of the course
    /// </summary>
    public readonly CourseDisplayView Course;

    /// <summary>
    /// Return if the user has an active lesson
    /// </summary>
    public bool EnrollmentIsActive => Model.IsActive;

    /// <summary>
    /// Returns the number of lessons in this enrollment
    /// </summary>
    public int EnrollmentLessonCount => Model.CourseRevision.Lessons.Count;

    /// <summary>
    /// Returns the number of lessons completed in this enrollment
    /// </summary>
    public int EnrollmentLessonCompletedCount => Model.LessonSubmissions.Count(l => l.IsFinal);

    /// <summary>
    /// The date the enrollment is closed.
    /// Either because the user ended his enrollment or did complete the course
    /// </summary>
    public DateTime? ClosureDate => Model.ClosureDate;

    /// <summary>
    /// Returns the progress (as percentage) of the student in this enrollment
    /// </summary>
    public float EnrollmentProgress
    {
        get
        {
            if (Model.IsCourseCompleted) return 100f;

            var completed = Model.LessonSubmissions.Count(l => l.IsFinal);
            var count = Model.CourseRevision.Lessons.Count;
            return (float)Math.Round((completed / (double)count) * 100.0, 2);
        }
    }

    /// <summary>
    /// Return the next lesson of the user
    /// </summary>
    public readonly string EnrollmentNextLesson;

    /// <summary>
    /// Return is the mentor still has lessons to review for this enrollment
    /// </summary>
    public readonly bool HasUnreviewedLessons;

    /// <summary>
    /// Returns the id of the mentor for this enrollment (can be different from current mentor)
    /// </summary>
    public readonly int? MentorId;

    /// <summary>
    /// Returns the name of the mentor for this enrollment (can be different from current mentor)
    /// </summary>
    public readonly string MentorName;

    /// <summary>
    /// Creates the view of the mentor on the student
    /// </summary>
    public StudentEnrollmentView(Enrollment enrollment, IFileStorageService fileStorageService) : base(enrollment)
    {
        // Deal with the progress of a student
        var sorted = enrollment.CourseRevision.Lessons.OrderBy(l => l.SequenceId).ToList();
        var completed = Model.LessonSubmissions.Count(l => l.IsFinal);
        if (completed < sorted.Count) EnrollmentNextLesson = sorted[completed].Name;

        if (enrollment.Mentor != null)
        {
            MentorId = enrollment.Mentor.Id;
            MentorName = enrollment.Mentor.Name;
        }

        // Determine if there are still lessons to validate
        HasUnreviewedLessons = enrollment.LessonSubmissions != null && enrollment.IsActive == false && enrollment.LessonSubmissions.Any(c => c.ReviewDate == null);
        Course = new CourseDisplayView(enrollment.CourseRevision.Course, fileStorageService);
    }
}