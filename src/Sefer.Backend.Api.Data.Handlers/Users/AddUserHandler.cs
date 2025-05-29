namespace Sefer.Backend.Api.Data.Handlers.Users;

public class AddUserHandler(IServiceProvider serviceProvider) : AddEntityHandler<AddUserRequest, User>(serviceProvider);