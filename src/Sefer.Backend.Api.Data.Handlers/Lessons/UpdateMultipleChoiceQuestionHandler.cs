namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class UpdateMultipleChoiceQuestionHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateMultipleChoiceQuestionRequest, MultipleChoiceQuestion>(serviceProvider);