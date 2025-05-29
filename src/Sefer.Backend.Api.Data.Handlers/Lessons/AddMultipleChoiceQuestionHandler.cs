namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class AddMultipleChoiceQuestionHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddMultipleChoiceQuestionRequest, MultipleChoiceQuestion>(serviceProvider);