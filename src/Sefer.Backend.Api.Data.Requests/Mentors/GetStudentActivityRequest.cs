namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetStudentActivityRequest(int mentorId) : IRequest<HashSet<int>>
{
    public readonly int MentorId = mentorId;
}