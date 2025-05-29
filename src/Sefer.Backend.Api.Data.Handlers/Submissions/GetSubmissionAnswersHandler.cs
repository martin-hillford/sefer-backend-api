namespace Sefer.Backend.Api.Data.Handlers.Submissions;

public class GetSubmissionAnswersHandler(IServiceProvider serviceProvider)
    : Handler<GetSubmissionAnswersRequest, List<QuestionAnswer>>(serviceProvider)
{
    public override async Task<List<QuestionAnswer>> Handle(GetSubmissionAnswersRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Answers.AsNoTracking().Where(a => a.SubmissionId == request.SubmissionId).ToListAsync(token);
    }
}