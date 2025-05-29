namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class PostEnrollmentChatMessageRequest(int enrollmentId) : IRequest<Message>
{
    public readonly int EnrollmentId = enrollmentId;
}