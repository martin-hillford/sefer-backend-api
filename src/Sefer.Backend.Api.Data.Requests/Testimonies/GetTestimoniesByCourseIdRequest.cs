namespace Sefer.Backend.Api.Data.Requests.Testimonies;

public class GetTestimoniesByCourseIdRequest(int courseId) : IRequest<List<Testimony>>
{
    public readonly int CourseId = courseId;
}