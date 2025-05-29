namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonsRequest(int? courseRevisionId) : IRequest<List<Lesson>>
{
    public readonly int? CourseRevisionId = courseRevisionId;
    
    public GetLessonsRequest(CourseRevision courseRevision)
        : this(courseRevision?.Id) { }
}