namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class IsValidLessonSubmissionPostRequest(int lessonSubmissionId) : IRequest<bool>
{
    public readonly int LessonSubmissionId = lessonSubmissionId;
}