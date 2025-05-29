namespace Sefer.Backend.Api.Data.Requests.Surveys;

public class GetSurveyResultsRequest(int? limit) : IRequest<List<SurveyResult>>
{
    public readonly int? Limit = limit;
}