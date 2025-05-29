namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmissionsForReviewCountRequest(int mentorId) : IRequest<long?>
{
    public readonly int MentorId = mentorId;
}