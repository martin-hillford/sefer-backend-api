namespace Sefer.Backend.Api.Data.Requests.Curricula;

public class GetCurriculumByPermalinkRequest(string permalink) : IRequest<Curriculum>
{
    public readonly string Permalink = permalink;
}