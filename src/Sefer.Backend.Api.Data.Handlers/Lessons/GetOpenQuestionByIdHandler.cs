namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetOpenQuestionByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetOpenQuestionByIdRequest, OpenQuestion>(serviceProvider);