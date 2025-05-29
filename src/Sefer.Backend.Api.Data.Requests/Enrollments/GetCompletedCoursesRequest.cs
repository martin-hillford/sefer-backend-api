namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetCompletedCoursesRequest : IRequest<List<Enrollment>>
{
    public readonly int StudentId;

    public GetCompletedCoursesRequest(int studentId)
    {
        StudentId = studentId;
    }
}
