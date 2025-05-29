namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class UpdateBoolQuestionHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateBoolQuestionRequest, BoolQuestion>(serviceProvider);