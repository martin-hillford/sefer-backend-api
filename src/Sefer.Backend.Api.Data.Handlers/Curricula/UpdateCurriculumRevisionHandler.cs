namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class UpdateCurriculumRevisionHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateCurriculumRevisionRequest, CurriculumRevision>(serviceProvider);