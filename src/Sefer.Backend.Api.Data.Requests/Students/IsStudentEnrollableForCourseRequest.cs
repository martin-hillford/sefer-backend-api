namespace Sefer.Backend.Api.Data.Requests.Students;

public class IsStudentEnrollableForCourseRequest : IRequest<bool>
{
    public readonly int CourseId;

    public readonly int StudentId;
    
    public IsStudentEnrollableForCourseRequest(User student, Course course)
    {
        StudentId = student.Id;
        CourseId = course.Id;
    }

    public IsStudentEnrollableForCourseRequest(int studentId, int courseId)
    {
        CourseId = courseId;
        StudentId = studentId;
    }
}