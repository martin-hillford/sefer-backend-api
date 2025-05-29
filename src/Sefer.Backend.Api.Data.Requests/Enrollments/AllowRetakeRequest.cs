namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class AllowRetakeRequest(int enrollmentId) : IRequest<bool>
{
    public readonly int EnrollmentId = enrollmentId;
}