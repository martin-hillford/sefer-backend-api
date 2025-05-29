namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class CurriculumValidationService(IServiceProvider serviceProvider)
    : CustomValidationService<Curriculum>(serviceProvider)
{
    public override async Task<bool> IsValid(Curriculum instance)
    {
        if (!await base.IsValid(instance)) return false;
        return await IsCurriculumPermalinkUnique(instance.Id, instance.Permalink);
    }

    private async Task<bool> IsCurriculumPermalinkUnique(int? courseId, string permalink)
    {
        var curriculum = await Send(new GetCurriculumByPermalinkRequest(permalink));
        return curriculum == null || curriculum.Id == courseId;
    }
}