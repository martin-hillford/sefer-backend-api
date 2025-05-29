namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetTakenCoursesOfStudentRequest(int studentId) : IRequest<List<Enrollment>>
{
    public readonly int StudentId = studentId;
}