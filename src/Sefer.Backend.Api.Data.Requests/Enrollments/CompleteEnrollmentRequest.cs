namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class CompleteEnrollmentRequest : IRequest<bool>
{
    public readonly int EnrollmentId;

    public CompleteEnrollmentRequest(int enrollmentId)
    {
        EnrollmentId = enrollmentId;
    }
}