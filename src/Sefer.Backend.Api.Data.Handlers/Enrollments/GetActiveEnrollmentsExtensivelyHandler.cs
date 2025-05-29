namespace Sefer.Backend.Api.Data.Handlers.Enrollments;

public class GetActiveEnrollmentsExtensivelyHandler(IServiceProvider serviceProvider)
    : Handler<GetActiveEnrollmentsExtensivelyRequest, List<EnrollmentSummary>>(serviceProvider)
{
    public override async Task<List<EnrollmentSummary>> Handle(GetActiveEnrollmentsExtensivelyRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.EnrollmentSummaries
            .Where(e => e.Rank == 1)
            .OrderByDescending(e => e.StudentLastActive)
            .ToListAsync(cancellationToken: token);
    }
}