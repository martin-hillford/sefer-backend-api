namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetActiveEnrollmentsOfStudentRequest(int studentId, bool extensively = false) : IRequest<List<Enrollment>>
{
    public readonly int StudentId = studentId;

    public readonly bool Extensively = extensively;
}