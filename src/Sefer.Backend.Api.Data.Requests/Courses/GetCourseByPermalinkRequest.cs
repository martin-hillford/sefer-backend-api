namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetCourseByPermalinkRequest(string permalink) : IRequest<Course>
{
    public readonly string Permalink = permalink;
}