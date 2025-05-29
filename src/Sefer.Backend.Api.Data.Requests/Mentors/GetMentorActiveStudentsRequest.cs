namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorActiveStudentsRequest(int mentorId, short? activeDays = null) : IRequest<List<Enrollment>>
{
    public readonly int MentorId = mentorId;

    public readonly short? ActiveDays = activeDays;
}