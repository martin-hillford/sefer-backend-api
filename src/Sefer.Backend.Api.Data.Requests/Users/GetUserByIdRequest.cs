namespace Sefer.Backend.Api.Data.Requests.Users;

public class GetUserByIdRequest(int? id) : GetEntityByIdRequest<User>(id);