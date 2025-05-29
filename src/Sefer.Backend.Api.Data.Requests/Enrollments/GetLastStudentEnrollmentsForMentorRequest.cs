namespace Sefer.Backend.Api.Data.Requests.Enrollments;

public class GetLastStudentEnrollmentsForMentorRequest(int mentorId) : IRequest<List<EnrollmentSummary>>
{
    public readonly int MentorId = mentorId;
}