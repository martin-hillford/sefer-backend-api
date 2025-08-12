namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetActiveEnrollmentOfStudentRequest(int studentId, bool extensively = false) : IRequest<Enrollment>
{
    public readonly int StudentId = studentId;

    public readonly bool Extensively = extensively;
}