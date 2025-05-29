namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class AddCurriculumBlockHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddCurriculumBlockRequest, CurriculumBlock>(serviceProvider);