namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmissionWithEnrollmentByIdRequest(int submissionId) : IRequest<LessonSubmission>
{
    public readonly int SubmissionId = submissionId;
}