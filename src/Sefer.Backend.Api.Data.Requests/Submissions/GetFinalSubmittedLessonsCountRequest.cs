namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetFinalSubmittedLessonsCountRequest(int enrollmentId) : IRequest<int>
{
    public readonly int EnrollmentId = enrollmentId;
}