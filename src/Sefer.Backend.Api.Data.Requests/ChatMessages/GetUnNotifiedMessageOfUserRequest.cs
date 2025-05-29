namespace Sefer.Backend.Api.Data.Requests.ChatMessages;

public class GetUnNotifiedMessageOfUserRequest(int user, int delay) : IRequest<List<Message>>
{
    public readonly int User = user;

    public readonly int Delay = delay;
}