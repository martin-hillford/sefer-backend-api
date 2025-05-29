namespace Sefer.Backend.Api.Support;

public abstract class UserController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    protected readonly IUserAuthenticationService UserAuthenticationService = 
        serviceProvider.GetService<IUserAuthenticationService>();

    protected int? UserId => UserAuthenticationService.UserId;
}