namespace Sefer.Backend.Api.Data.Requests.Students;

public class SetPersonalMentorRequest(int studentId, int mentorId) : IRequest<bool>
{
    public readonly int StudentId = studentId;

    public readonly int MentorId = mentorId;
}