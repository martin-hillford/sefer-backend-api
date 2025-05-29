namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class GetSurveyResultByIdExtensiveHandler(IServiceProvider serviceProvider)
    : Handler<GetSurveyResultByIdExtensiveRequest, SurveyResult>(serviceProvider)
{
    public override async Task<SurveyResult> Handle(GetSurveyResultByIdExtensiveRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.SurveyResults
            .AsNoTracking()
            .Where(r => r.Id == request.SurveyResultId)
            .Include(r => r.Enrollment).ThenInclude(e => e.Mentor)
            .Include(r => r.Enrollment).ThenInclude(e => e.Student)
            .Include(r => r.Enrollment).ThenInclude(e => e.CourseRevision).ThenInclude(r => r.Course)
            .Include(r => r.MentorRating)
            .Include(r => r.CourseRating)
            .FirstOrDefaultAsync(token);
    }
}