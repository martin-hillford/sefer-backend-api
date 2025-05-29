namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class UpdateCurriculumBlockHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateCurriculumBlockRequest, CurriculumBlock>(serviceProvider);