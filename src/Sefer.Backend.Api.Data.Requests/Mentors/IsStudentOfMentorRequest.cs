namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class IsStudentOfMentorRequest(int mentorId, int studentId) : IRequest<bool>
{
    public readonly int StudentId = studentId;

    public readonly int MentorId = mentorId;
}