namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetLastClosedEnrollmentRequest(int studentId) : IRequest<Enrollment>
{
    public readonly int StudentId = studentId;
}