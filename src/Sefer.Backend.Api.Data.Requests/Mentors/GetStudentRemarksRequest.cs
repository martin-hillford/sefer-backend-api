namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetStudentRemarksRequest(int mentorId, int studentId) : IRequest<MentorStudentData>
{
    public readonly int MentorId = mentorId;

    public readonly int StudentId = studentId;
}