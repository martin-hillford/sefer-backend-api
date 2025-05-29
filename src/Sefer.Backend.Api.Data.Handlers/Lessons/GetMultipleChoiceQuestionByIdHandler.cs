namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetMultipleChoiceQuestionByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetMultipleChoiceQuestionByIdRequest, MultipleChoiceQuestion>(serviceProvider);