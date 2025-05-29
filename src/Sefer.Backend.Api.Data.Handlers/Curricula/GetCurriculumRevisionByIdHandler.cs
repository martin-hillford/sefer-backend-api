namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class GetCurriculumRevisionByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetCurriculumRevisionByIdRequest, CurriculumRevision>(serviceProvider);