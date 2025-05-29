namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class IsSubmissionReviewableRequest(int mentorId, int submissionId) : IRequest<bool>
{
    public readonly int MentorId = mentorId;

    public readonly int SubmissionId = submissionId;
}