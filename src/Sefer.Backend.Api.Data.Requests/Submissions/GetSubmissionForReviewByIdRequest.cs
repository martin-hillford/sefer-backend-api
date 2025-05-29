namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmissionForReviewByIdRequest(int mentorId, int submissionId) : IRequest<LessonSubmission>
{
    public readonly int MentorId = mentorId;

    public readonly int SubmissionId = submissionId;
}