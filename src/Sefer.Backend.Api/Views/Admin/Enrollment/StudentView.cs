// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Data.JsonViews;
using DbEnrollment = Sefer.Backend.Api.Data.Models.Enrollments.Enrollment;
using DbUser = Sefer.Backend.Api.Data.Models.Users.User;

namespace Sefer.Backend.Api.Views.Admin.Enrollment;

/// <summary>
/// A view on all the enrollments of a student
/// </summary>
public class StudentView
{
    #region Properties

    public StudentView(DbUser student, List<DbEnrollment> enrollments, ICryptographyService cryptographyService, IFileStorageService fileStorageService)
    {
        Student = new UserView(student);
        Enrollments = enrollments.Select(e => new EnrollmentView(e, cryptographyService, fileStorageService));
    }

    #endregion

    #region Properties

    /// <summary>
    /// The student for which the enrollments are loaded
    /// </summary>
    public readonly UserView Student;

    /// <summary>
    /// The enrollments the student
    /// </summary>
    public readonly IEnumerable<EnrollmentView> Enrollments;

    #endregion
}