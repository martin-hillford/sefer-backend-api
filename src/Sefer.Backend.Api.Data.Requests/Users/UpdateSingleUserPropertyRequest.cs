namespace Sefer.Backend.Api.Data.Requests.Users;

public class UpdateSingleUserPropertyRequest(User entity, string property)
    : UpdateSingleEntityPropertyRequest<User>(entity, property);