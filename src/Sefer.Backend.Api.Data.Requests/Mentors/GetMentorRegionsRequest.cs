namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorRegionsRequest(int mentorId) : IRequest<List<string>>
{
    public readonly int MentorId = mentorId;
}