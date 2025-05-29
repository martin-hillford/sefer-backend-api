namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetLastStudentEnrollmentsForMentorHandler(IServiceProvider serviceProvider)
    : Handler<GetLastStudentEnrollmentsForMentorRequest, List<EnrollmentSummary>>(serviceProvider)
{
    public override async Task<List<EnrollmentSummary>> Handle(GetLastStudentEnrollmentsForMentorRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.EnrollmentSummaries
            .Where(e => e.MentorId == request.MentorId && e.Rank == 1)
            .OrderByDescending(e => e.StudentLastActive)
            .ToListAsync(cancellationToken: token);
    }
}