namespace Sefer.Backend.Api.Data.Requests.Surveys;

public class GetSurveyByIdRequest(int? id) : GetEntityByIdRequest<Survey>(id);