namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class DeleteMultipleChoiceQuestionHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteMultipleChoiceQuestionRequest, MultipleChoiceQuestion>(serviceProvider);