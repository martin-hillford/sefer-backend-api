namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumBlockByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetCurriculumBlockByIdRequest, CurriculumBlock>(serviceProvider);