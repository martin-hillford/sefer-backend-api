using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// The view of the mentor on the student
/// </summary>
public class StudentOfMentorView : MentorStudentInfoView
{
    /// <summary>
    /// A list of enrollments into course the user has done
    /// </summary>
    public readonly IReadOnlyCollection<StudentEnrollmentView> Enrollments;

    /// <summary>
    /// Holds the current enrollment
    /// </summary>
    public readonly EnrollmentSummary Current;

    /// <summary>
    /// Holds if the student is active
    /// </summary>
    public readonly bool IsActive;

    /// <summary>
    /// Creates the view of the mentor on the student
    /// </summary>
    public StudentOfMentorView(User student, bool isActive, User mentor, MentorStudentData data, EnrollmentSummary summary, IFileStorageService fileStorageService, IAvatarService avatarService) : base(student, data, avatarService)
    {
        var enrollments = new List<StudentEnrollmentView>();

        student.Enrollments?.Where(e => e.MentorId == mentor.Id).OrderByDescending(e => e.CreationDate).ToList().ForEach(e =>
        {
            e.Student = student;
            enrollments.Add(new StudentEnrollmentView(e, fileStorageService));
        });

        Enrollments = enrollments.AsReadOnly();
        Current = summary;
        IsActive = isActive;
    }
}