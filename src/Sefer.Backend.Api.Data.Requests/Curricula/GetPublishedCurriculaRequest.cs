namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetPublishedCurriculaRequest(bool includeCourses = false) : IRequest<List<Curriculum>>
{
    public readonly bool IncludeCourses = includeCourses;
}