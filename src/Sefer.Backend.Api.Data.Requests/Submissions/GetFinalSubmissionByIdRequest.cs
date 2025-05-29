namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetFinalSubmissionByIdRequest(int submissionId) : IRequest<LessonSubmission>
{
    public readonly int SubmissionId = submissionId;
}