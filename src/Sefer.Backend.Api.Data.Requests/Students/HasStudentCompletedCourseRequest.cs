namespace Sefer.Backend.Api.Data.Requests.Students;

public class HasStudentCompletedCourseRequest(int studentId, int courseId) : IRequest<bool>
{
    public readonly int CourseId = courseId;

    public readonly int StudentId = studentId;
}