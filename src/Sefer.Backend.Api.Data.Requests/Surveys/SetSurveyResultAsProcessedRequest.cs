namespace Sefer.Backend.Api.Data.Requests.Surveys;

public class SetSurveyResultAsProcessedRequest(int surveyResultId) : IRequest<bool>
{
    public readonly int SurveyResultId = surveyResultId;
}