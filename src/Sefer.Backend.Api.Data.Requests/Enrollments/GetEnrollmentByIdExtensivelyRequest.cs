namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetEnrollmentByIdExtensivelyRequest(int? enrollmentId) : IRequest<Enrollment>
{
    public readonly int? EnrollmentId = enrollmentId;
}