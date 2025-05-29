namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class UserValidationService(IServiceProvider serviceProvider)
    : CustomValidationService<User>(serviceProvider)
{
    public override async Task<bool> IsValid(User instance)
    {
        if (!await base.IsValid(instance)) return false;
        var user = await Send(new GetUserByEmailRequest(instance.Email));
        return user == null || user.Id == instance.Id;
    }
}