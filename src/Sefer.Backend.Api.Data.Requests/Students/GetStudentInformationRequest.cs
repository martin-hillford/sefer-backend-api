namespace Sefer.Backend.Api.Data.Requests.Students;

public class GetStudentInformationRequest(int studentId) : IRequest<(User Student, EnrollmentSummary Current, bool IsActive)?>
{
    public readonly int StudentId = studentId;
}