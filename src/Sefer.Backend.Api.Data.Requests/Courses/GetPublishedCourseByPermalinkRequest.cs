namespace Sefer.Backend.Api.Data.Requests.Courses;

public class GetPublishedCourseByPermalinkRequest(string permalink) : IRequest<Course>
{
    public readonly string Permalink = permalink;
}