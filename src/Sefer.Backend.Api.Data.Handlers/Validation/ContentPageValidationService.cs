namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class ContentPageValidationService(IServiceProvider serviceProvider)
    : CustomValidationService<ContentPage>(serviceProvider)
{
    public override async Task<bool> IsValid(ContentPage instance)
    {
        if (!await base.IsValid(instance)) return false;
        var permalink = instance?.Permalink.Trim();
        if (string.IsNullOrEmpty(permalink)) return true;
        if (!await Send(new IsContentPageNameUniqueRequest(instance.Id, instance.Name))) return false;

        var permalinkPage = await Send(new GetContentPageByPermalinkRequest(instance.Permalink));
        return permalinkPage == null || permalinkPage.Id == instance.Id;
    }
}