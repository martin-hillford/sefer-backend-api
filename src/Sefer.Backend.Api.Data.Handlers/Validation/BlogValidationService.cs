namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class BlogValidationService(IServiceProvider serviceProvider) : CustomValidationService<Blog>(serviceProvider)
{
    public override async Task<bool> IsValid(Blog instance)
    {
        if (!await base.IsValid(instance)) return false;

        return
            await Send(new IsBlogNameUniqueRequest(instance.Id, instance.Name)) &&
            await Send(new IsBlogPermalinkUniqueRequest(instance.Id, instance.Permalink));
    }
}