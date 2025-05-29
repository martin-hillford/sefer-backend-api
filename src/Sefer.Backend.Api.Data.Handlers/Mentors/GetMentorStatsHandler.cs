namespace Sefer.Backend.Api.Data.Handlers.Mentors;

public class GetMentorStatsHandler(IServiceProvider serviceProvider)
    : Handler<GetMentorStatsRequest, MentorStats>(serviceProvider)
{
    public override async Task<MentorStats> Handle(GetMentorStatsRequest request, CancellationToken token)
    {
        var context = GetDataContext();

        var totalStudents = await context.Enrollments
            .CountAsync(e => e.MentorId == request.MentorId, token);

        var totalCourses = await context.Enrollments
            .CountAsync(e => e.MentorId == request.MentorId && e.IsCourseCompleted, token);

        var totalLessons = await context.LessonSubmissions
            .CountAsync(e => e.ResultsStudentVisible && e.IsFinal && e.Enrollment.MentorId == request.MentorId, token);

        return new MentorStats
        {
            TotalStudents = totalStudents,
            TotalReviewedCourses = totalCourses,
            TotalReviewedLessons = totalLessons
        };
    }
}