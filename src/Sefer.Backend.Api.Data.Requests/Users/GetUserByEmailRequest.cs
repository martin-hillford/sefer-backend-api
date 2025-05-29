namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetUserByEmailRequest(string email) : IRequest<User>
{
    public readonly string Email = email;
}