namespace Sefer.Backend.Api.Data.Requests.Students;

public class GetPersonalMentorForCourseRequest(int studentId, int courseId) : IRequest<User>
{
    public readonly int StudentId = studentId;

    public readonly int CourseId = courseId;
}