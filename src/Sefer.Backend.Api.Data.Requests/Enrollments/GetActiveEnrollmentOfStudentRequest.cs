namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetActiveEnrollmentOfStudentRequest : IRequest<Enrollment>
{
    public readonly int StudentId;

    public readonly bool Extensively;
    
    public GetActiveEnrollmentOfStudentRequest(int studentId, bool extensively = false)
    {
        StudentId = studentId;
        Extensively = extensively;
    }
}