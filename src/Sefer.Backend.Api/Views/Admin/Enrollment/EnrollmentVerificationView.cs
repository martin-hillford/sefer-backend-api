// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;

namespace Sefer.Backend.Api.Views.Admin.Enrollment;

/// <summary>
/// This is a special view that is created to verify an enrollment
/// </summary>
public class EnrollmentVerificationView
{
    /// <summary>
    /// The date the enrollment was closed
    /// </summary>
    public readonly DateTime? ClosureDate;

    /// <summary>
    /// The course the enrollment was about
    /// </summary>
    public readonly CourseView Course;

    /// <summary>
    /// A url to the diploma
    /// </summary>
    public readonly string DiplomaUrl;

    /// <summary>
    /// The student that took the course
    /// </summary>
    public readonly UserView Student;

    /// <summary>
    /// Creates a new enrollment verification view
    /// </summary>
    /// <param name="enrollment"></param>
    /// <param name="cryptographyService"></param>
    public EnrollmentVerificationView(Data.Models.Enrollments.Enrollment enrollment, ICryptographyService cryptographyService)
    {
        ClosureDate = enrollment.ClosureDate;
        Course = new CourseView(enrollment.CourseRevision.Course);
        Student = new UserView(enrollment.Student);

        var hash = cryptographyService.Hash(enrollment.Id.ToString());
        hash = BitConverter.ToString(Convert.FromBase64String(hash)).Replace("-", string.Empty).ToLower();
        DiplomaUrl = $"/student/enrollments/{enrollment.Id}/diploma/{hash}";
    }
}