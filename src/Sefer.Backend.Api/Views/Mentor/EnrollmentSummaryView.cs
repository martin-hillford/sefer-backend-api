// This is view, so property may not be accessed in code
// ReSharper disable UnusedMember.Global MemberCanBePrivate.Global NotAccessedField.Global
using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Mentor;

/// <summary>
/// A full view on an enrollment of a student from the perspective of a mentor
/// </summary>
public class EnrollmentSummaryView : EnrollmentSummary
{
    /// <summary>
    /// The avatar url for the student
    /// </summary>
    public readonly string StudentAvatarUrl;

    public EnrollmentSummaryView(EnrollmentSummary summary, IAvatarService avatarService)
    {
        Id = summary.Id;
        CreationDate = summary.CreationDate;
        StudentId = summary.StudentId;
        StudentName = summary.StudentName;
        MentorId = summary.MentorId;
        MentorName = summary.MentorName;
        CourseRevisionId = summary.CourseRevisionId;
        CourseId = summary.CourseId;
        CourseName = summary.CourseName;
        Submitted = summary.Submitted;
        LessonCount = summary.LessonCount;
        NextLessonId = summary.NextLessonId;
        NextLessonName = summary.NextLessonName;
        StudentLastActive = summary.StudentLastActive;
        StudentAvatarUrl = avatarService.GetAvatarUrl(StudentId, StudentName);
    }
}