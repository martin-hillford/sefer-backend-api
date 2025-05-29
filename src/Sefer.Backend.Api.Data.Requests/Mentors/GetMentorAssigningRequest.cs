namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorAssigningRequest(int courseId, int studentId) : IRequest<User>
{
    public readonly int CourseId = courseId;

    public readonly int StudentId = studentId;
}