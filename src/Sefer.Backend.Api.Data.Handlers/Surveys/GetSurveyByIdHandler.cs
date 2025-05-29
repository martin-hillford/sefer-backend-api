namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class GetSurveyByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetSurveyByIdRequest, Survey>(serviceProvider);