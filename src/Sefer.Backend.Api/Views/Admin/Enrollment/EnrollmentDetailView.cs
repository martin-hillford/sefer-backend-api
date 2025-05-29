// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global, UnusedAutoPropertyAccessor.Global
using Sefer.Backend.Api.Controllers.Mentor;
using Sefer.Backend.Api.Data.JsonViews;
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Mentor;

namespace Sefer.Backend.Api.Views.Admin.Enrollment;

/// <summary>
/// This is the detailed view on an enrollment include with all submitted lessons
/// </summary>
public class EnrollmentDetailView : EnrollmentView
{
    /// <summary>
    /// The student for which the enrollments are loaded
    /// </summary>
    public readonly UserView Student;

    /// <summary>
    /// Holds a list of all the submissions of lessons in this enrollment
    /// </summary>
    public readonly IEnumerable<ReviewSummaryView> Submissions;

    public EnrollmentDetailView(Data.Models.Enrollments.Enrollment enrollment, ICryptographyService cryptographyService, IFileStorageService fileStorageService, IAvatarService avatarService)
        : base(enrollment, cryptographyService, fileStorageService)
    {

        Submissions = enrollment.LessonSubmissions.Select(s =>
        {
            var answers = SubmissionsController.GetAnswersForSubmissionView(s);
            return new ReviewSummaryView(s, answers, null, avatarService);
        });

        Student = new UserView(enrollment.Student);
    }
}