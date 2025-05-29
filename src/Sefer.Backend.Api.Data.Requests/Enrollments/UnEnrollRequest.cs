namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class UnEnrollRequest(int enrollmentId) : IRequest<bool>
{
    public readonly int EnrollmentId = enrollmentId;
}