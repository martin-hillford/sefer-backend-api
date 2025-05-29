namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmissionAnswersRequest(int submissionId) : IRequest<List<QuestionAnswer>>
{
    public readonly int SubmissionId = submissionId;
}