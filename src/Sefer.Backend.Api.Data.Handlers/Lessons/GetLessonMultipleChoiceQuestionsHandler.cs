namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonMultipleChoiceQuestionsHandler(IServiceProvider serviceProvider)
    : Handler<GetLessonMultipleChoiceQuestionsRequest, List<MultipleChoiceQuestion>>(serviceProvider)
{
    public override async Task<List<MultipleChoiceQuestion>> Handle(GetLessonMultipleChoiceQuestionsRequest request, CancellationToken token)
    {
        var context = GetDataContext();

        return await context.LessonMultipleChoiceQuestions
            .Where(m => m.LessonId == request.LessonId)
            .Include(m => m.Choices)
            .OrderBy(m => m.SequenceId)
            .ToListAsync(token);
    }
}