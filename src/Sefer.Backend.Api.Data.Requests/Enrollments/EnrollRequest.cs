namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class EnrollRequest : IRequest<Enrollment>
{
    public readonly int StudentId;

    public readonly int CourseId;

    public EnrollRequest(int studentId, int courseId)
    {
        CourseId = courseId;
        StudentId = studentId;
    }
}