namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmissionsForReviewRequest(int mentorId) : IRequest<List<LessonSubmission>>
{
    public readonly int MentorId = mentorId;
}