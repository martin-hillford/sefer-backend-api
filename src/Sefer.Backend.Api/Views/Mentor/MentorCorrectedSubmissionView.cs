using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Student.SubmissionOverview;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// A view for the mentor with the correct submission
/// </summary>
public class MentorCorrectedSubmissionView : CorrectedSubmissionResultView
{
    /// <summary>
    /// Details about the student
    /// </summary>
    public readonly MentorStudentInfoView Student;

    /// <summary>
    /// Creates a new view, this view will include a list of answers
    /// </summary>
    /// <inheritdoc />
    public MentorCorrectedSubmissionView(LessonSubmission submission, MentorStudentData data, IFileStorageService fileStorageService, IAvatarService avatarService) : base(submission, fileStorageService)
    {
        Student = new MentorStudentInfoView(submission.Enrollment.Student, data, avatarService);
    }
}