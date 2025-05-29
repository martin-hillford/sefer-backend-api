namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class UpdateSurveyHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateSurveyRequest, Survey>(serviceProvider);