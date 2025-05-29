namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class EnsureMentorSettingsRequest(int mentorId) : IRequest<MentorSettings>
{
    public readonly int MentorId = mentorId;
}