namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class AddSurveyHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddSurveyRequest, Survey>(serviceProvider);

