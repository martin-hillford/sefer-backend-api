namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class UpdateOpenQuestionHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateOpenQuestionRequest, OpenQuestion>(serviceProvider);