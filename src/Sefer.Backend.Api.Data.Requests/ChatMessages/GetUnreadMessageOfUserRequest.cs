namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class GetUnreadMessageOfUserRequest(int user) : IRequest<List<Message>>
{
    public readonly int User = user;
}