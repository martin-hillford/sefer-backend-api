namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonChoicesByQuestionIdHandler(IServiceProvider serviceProvider)
    : Handler<GetLessonChoicesByQuestionIdRequest, List<MultipleChoiceQuestionChoice>>(serviceProvider)
{
    public override async Task<List<MultipleChoiceQuestionChoice>> Handle(GetLessonChoicesByQuestionIdRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.LessonMultipleChoiceQuestionChoices
            .AsNoTracking()
            .Where(c => c.QuestionId == request.QuestionId)
            .ToListAsync(token);
    }
}