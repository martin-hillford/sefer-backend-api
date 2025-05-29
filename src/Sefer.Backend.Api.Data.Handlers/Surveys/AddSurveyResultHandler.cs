namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class AddSurveyResultHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddSurveyResultRequest, SurveyResult>(serviceProvider);