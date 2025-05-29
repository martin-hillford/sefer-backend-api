namespace Sefer.Backend.Api.Data.Requests.Students;

public class DeleteStudentRequest(int studentId) : IRequest<bool>
{
    public readonly int StudentId = studentId;
}