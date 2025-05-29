namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class CalculateSubmissionGradeRequest(int submissionId) : IRequest<double?>
{
    public readonly int SubmissionId = submissionId;
}