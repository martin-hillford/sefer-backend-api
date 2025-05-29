namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class GetSurveyResultsHandler(IServiceProvider serviceProvider)
    : Handler<GetSurveyResultsRequest, List<SurveyResult>>(serviceProvider)
{
    public override async Task<List<SurveyResult>> Handle(GetSurveyResultsRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.SurveyResults
            .AsNoTracking()
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