namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class DeleteBoolQuestionHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteBoolQuestionRequest, BoolQuestion>(serviceProvider);