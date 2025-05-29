namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class RemoveAsMentorRequest(int mentorId) : IRequest<bool>
{
    public readonly int MentorId = mentorId;
}