namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class PostSubmissionMessageRequest(int lessonSubmissionId) : IRequest<Message>
{
    public readonly int LessonSubmissionId = lessonSubmissionId;
}