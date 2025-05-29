namespace Sefer.Backend.Api.Data.Handlers.Surveys;

public class GetSurveyByCourseRevisionHandler(IServiceProvider serviceProvider)
    : Handler<GetSurveyByCourseRevisionRequest, Survey>(serviceProvider)
{
    public override async Task<Survey> Handle(GetSurveyByCourseRevisionRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var revision = await context.CourseRevisions.SingleOrDefaultAsync(c => c.Id == request.CourseRevisionId, token);
        if (revision == null) return null;

        var survey = await context.Surveys.FirstOrDefaultAsync(s => s.CourseRevisionId == revision.Id, token);
        if (survey != null && revision.AllowSelfStudy) survey.EnableMentorRating = false;
        return survey;
    }
}