namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class AddBoolQuestionHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddBoolQuestionRequest, BoolQuestion>(serviceProvider);