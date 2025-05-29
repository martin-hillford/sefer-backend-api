namespace Sefer.Backend.Api.Data.Requests.Students;

public class RemoveStudentRoleRequest(int studentId) : IRequest<bool>
{
    public readonly int StudentId = studentId;
}