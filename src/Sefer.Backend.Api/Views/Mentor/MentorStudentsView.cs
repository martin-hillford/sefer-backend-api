using Sefer.Backend.Api.Services.Avatars;

namespace Sefer.Backend.Api.Views.Mentor;

public class MentorStudentsView
{
    public readonly List<EnrollmentSummaryView> Active = new();

    public readonly List<EnrollmentSummaryView> Inactive = new();

    public MentorStudentsView(List<EnrollmentSummary> enrollments, DateTime active, IAvatarService avatarService)
    {
        foreach (var enrollment in enrollments)
        {
            var view = new EnrollmentSummaryView(enrollment, avatarService);
            if (enrollment.StudentLastActive >= active) Active.Add(view);
            else Inactive.Add(view);
        }
    }
}