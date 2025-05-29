using Sefer.Backend.Api.Views.Shared.Courses;

namespace Sefer.Backend.Api.Views.Admin.Course;

/// <summary>
/// This course view is special for admins since it will include the maximum of submittable lesson for a day
/// </summary>
public class CourseView : CourseDisplayView
{
    /// <summary>
    /// The maximum number of lesson to submit per day for this course per student
    /// </summary>
    public int? MaxLessonSubmissionsPerDay => Model.MaxLessonSubmissionsPerDay;

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="model">The model of the view</param>
    /// <param name="fileStorageService">A service that is storing files</param>
    /// <inheritdoc />
    public CourseView(Data.Models.Courses.Course model, IFileStorageService fileStorageService)
        : base(model, fileStorageService) { }
}