namespace Sefer.Backend.Api.Data.Requests.Surveys;

public class GetSurveyByCourseRevisionRequest(int courseRevisionId) : IRequest<Survey>
{
    public readonly int CourseRevisionId = courseRevisionId;
}