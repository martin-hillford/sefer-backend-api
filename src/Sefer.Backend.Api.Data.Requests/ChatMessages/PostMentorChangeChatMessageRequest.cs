namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class PostMentorChangeChatMessageRequest : IRequest<List<Message>>
{
    public string CourseName { get; init; }

    public int Student { get; init; }

    public int OldMentor { get; init; }

    public int NewMentor { get; init; }
}