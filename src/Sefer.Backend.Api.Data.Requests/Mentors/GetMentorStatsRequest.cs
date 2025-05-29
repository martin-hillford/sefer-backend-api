namespace Sefer.Backend.Api.Data.Requests.Mentors;

public class GetMentorStatsRequest(int mentorId) : IRequest<MentorStats>
{
    public readonly int MentorId = mentorId;
}

public class MentorStats
{
    public long TotalStudents { get; init; }

    public long TotalReviewedLessons { get; init; }

    public long TotalReviewedCourses { get; init; }
}