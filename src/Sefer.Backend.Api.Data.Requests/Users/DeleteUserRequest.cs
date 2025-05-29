namespace Sefer.Backend.Api.Data.Requests.Users;

public class DeleteUserRequest(User entity) : DeleteEntityRequest<User>(entity);