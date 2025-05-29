namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class GetUnprocessedSurveyResultsHandler(IServiceProvider serviceProvider)
    : Handler<GetUnprocessedSurveyResultsRequest, List<SurveyResult>>(serviceProvider)
{
    public override async Task<List<SurveyResult>> Handle(GetUnprocessedSurveyResultsRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.SurveyResults
            .AsNoTracking()
            .Where(r => r.AdminProcessed == false)
            .Include(r => r.Enrollment).ThenInclude(e => e.Mentor)
            .Include(r => r.Enrollment).ThenInclude(e => e.Student)
            .Include(r => r.Enrollment).ThenInclude(e => e.CourseRevision).ThenInclude(r => r.Course)
            .Include(r => r.MentorRating)
            .Include(r => r.CourseRating)
            .OrderByDescending(c => c.CreationDate).ThenByDescending(c => c.Id)
            .Limit(request.Limit)
            .ToListAsync(token);
    }
}