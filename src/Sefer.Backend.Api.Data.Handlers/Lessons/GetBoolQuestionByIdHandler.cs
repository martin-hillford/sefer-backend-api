namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetBoolQuestionByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetBoolQuestionByIdRequest, BoolQuestion>(serviceProvider);