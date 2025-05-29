namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class IsChoiceValidHandler(IServiceProvider serviceProvider)
    : IsValidEntityHandler<IsChoiceValidRequest, MultipleChoiceQuestionChoice>(serviceProvider);