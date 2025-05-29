namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class PostReviewChatMessageRequest(int lessonSubmissionId) : IRequest<List<Message>>
{
    public readonly int LessonSubmissionId = lessonSubmissionId;
}