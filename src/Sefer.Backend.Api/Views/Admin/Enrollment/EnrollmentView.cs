// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Views.Shared.Enrollments;

namespace Sefer.Backend.Api.Views.Admin.Enrollment;

/// <summary>
/// This is the extended view for the admin on an enrollment
/// </summary>
public class EnrollmentView : ExtendedEnrollmentView
{
    #region Properties

    /// <summary>
    /// Holds if the user will have a diploma for this course
    /// </summary>
    public readonly bool HasDiploma;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new View
    /// </summary>
    /// <param name="enrollment">The enrollment of the view</param>
    /// <param name="cryptographyService">The service to hash</param>
    /// <param name="fileStorageService">The service to helps with image urls</param>
    /// <inheritdoc />
    public EnrollmentView(Data.Models.Enrollments.Enrollment enrollment, ICryptographyService cryptographyService, IFileStorageService fileStorageService) : base(enrollment, fileStorageService)
    {
        if (enrollment.Grade.HasValue) Grade = Math.Round(enrollment.Grade.Value * 10, 1);
        else if (enrollment.LessonSubmissions != null && enrollment.IsCourseCompleted)
        {
            var sum = enrollment.LessonSubmissions.Where(l => l.Grade.HasValue).Select(l => l.Grade.Value).DefaultIfEmpty(0).Sum();
            Grade = Math.Round(sum / LessonCount * 10, 1);
        }

        var hash = cryptographyService.Hash(enrollment.Id.ToString());
        hash = BitConverter.ToString(Convert.FromBase64String(hash)).Replace("-", string.Empty).ToLower();
        HasDiploma = IsCourseCompleted && Grade is >= 7;
        if (HasDiploma) DiplomaUrl = $"/student/enrollments/{enrollment.Id}/diploma/{hash}";
    }

    #endregion

    #region Properties

    /// <summary>
    /// The grade of the student for the course
    /// </summary>
    public readonly double? Grade;

    /// <summary>
    /// This property will contain if the student can re-enroll for an enrollment (given no active enrollment)
    /// </summary>
    public bool CanStudentReEnroll => (Model.AllowRetake || Model.IsCourseCompleted == false) && IsActive == false;

    /// <summary>
    /// The url to the diploma
    /// </summary>
    public readonly string DiplomaUrl;

    #endregion
}