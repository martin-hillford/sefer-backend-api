namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetCurrentLessonRequest(int studentId, int enrollmentId) : IRequest<(Lesson, LessonSubmission, Enrollment)>
{
    public readonly int StudentId = studentId;
    
    public readonly int EnrollmentId = enrollmentId;
}