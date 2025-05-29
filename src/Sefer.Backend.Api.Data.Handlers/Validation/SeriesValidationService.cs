namespace Sefer.Backend.Api.Data.Handlers.Validation;

public class SeriesValidationService(IServiceProvider serviceProvider)
    : CustomValidationService<Series>(serviceProvider)
{
    public override async Task<bool> IsValid(Series instance)
    {
        if (!await base.IsValid(instance)) return false;
        return await Send(new IsSeriesNameUniqueRequest(instance.Id, instance.Name));
    }
}