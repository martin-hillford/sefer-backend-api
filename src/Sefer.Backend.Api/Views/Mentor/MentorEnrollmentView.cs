// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Services.Avatars;
using Sefer.Backend.Api.Views.Student.SubmissionOverview;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// A full view on an enrollment of a student from the perspective of a mentor
/// </summary>
public class MentorEnrollmentView : StudentEnrollmentView
{
    /// <summary>
    /// A list of submission in the enrollment
    /// </summary>
    public readonly ReadOnlyCollection<CorrectedSubmissionResultView> Submissions;

    /// <summary>
    /// Details about the student
    /// </summary>
    public readonly MentorStudentInfoView Student;

    /// <summary>
    /// The grade the student got for the course (only set when complete)
    /// </summary>
    public double? Grade => Model.Grade;

    /// <summary>
    /// Holds if there is a grade for the course
    /// </summary>
    public bool HasGrade => Model.Grade.HasValue;

    /// <summary>
    /// Creates a full view on an enrollment of a student
    /// </summary>
    public MentorEnrollmentView(Enrollment enrollment, IFileStorageService fileStorageService, MentorStudentData data, IAvatarService avatarService) : base(enrollment, fileStorageService)
    {
        Student = new MentorStudentInfoView(enrollment.Student, data, avatarService);
        Submissions = enrollment.LessonSubmissions
            .Where(l => l.IsFinal)
            .OrderByDescending(s => s.SubmissionDate)
            .Select(sub => new CorrectedSubmissionResultView(sub, fileStorageService))
            .ToList().AsReadOnly();
    }
}