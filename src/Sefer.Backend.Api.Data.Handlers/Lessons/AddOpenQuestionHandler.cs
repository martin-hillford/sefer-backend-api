namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class AddOpenQuestionHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddOpenQuestionRequest, OpenQuestion>(serviceProvider);