namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetEnrollmentsOfStudentRequest(int userId, int? top = null, bool extensive = false)
    : IRequest<List<Enrollment>>
{
    public readonly int UserId = userId;

    public readonly int? Top = top;

    public readonly bool Extensive = extensive;
}