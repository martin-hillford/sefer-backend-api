namespace Sefer.Backend.Api.Data.Requests.Students;

public class GetPersonalMentorRequest(int studentId) : IRequest<User>
{
    public readonly int StudentId = studentId;
}