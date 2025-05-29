namespace Sefer.Backend.Api.Data.Requests.Surveys;

public class GetUnprocessedSurveyResultsRequest(int? limit) : IRequest<List<SurveyResult>>
{
    public readonly int? Limit = limit;
}