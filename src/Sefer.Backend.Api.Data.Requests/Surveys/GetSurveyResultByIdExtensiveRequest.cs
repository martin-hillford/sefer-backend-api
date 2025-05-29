namespace Sefer.Backend.Api.Data.Requests.Surveys;

public class GetSurveyResultByIdExtensiveRequest(int surveyResultId) : IRequest<SurveyResult>
{
    public readonly int SurveyResultId = surveyResultId;
}