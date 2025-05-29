namespace Sefer.Backend.Api.Data.Requests.Submissions;

public class GetSubmittedLessonsRequest(int userId, uint top) : IRequest<List<LessonSubmission>>
{
    public readonly int UserId = userId;

    public readonly uint Top = top;
}