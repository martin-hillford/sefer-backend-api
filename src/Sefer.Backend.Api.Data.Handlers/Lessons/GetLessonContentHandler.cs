namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonContentHandler(IServiceProvider serviceProvider)
    : Handler<GetLessonContentRequest, List<IContentBlock<Lesson>>>(serviceProvider)
{
    public override async Task<List<IContentBlock<Lesson>>> Handle(GetLessonContentRequest request, CancellationToken token)
    {
        var blocks = new List<IContentBlock<Lesson>>();
        if (request.Lesson == null) return blocks;

        blocks.AddRange(await Send(new GetLessonOpenQuestionsRequest(request.Lesson), token));
        blocks.AddRange(await Send(new GetLessonMediaElementsRequest(request.Lesson), token));
        blocks.AddRange(await Send(new GetLessonTextElementsRequest(request.Lesson), token));
        blocks.AddRange(await Send(new GetLessonBoolQuestionsRequest(request.Lesson), token));
        blocks.AddRange(await Send(new GetLessonMultipleChoiceQuestionsRequest(request.Lesson), token));

        return blocks.OrderBy(b => b.SequenceId).ToList();
    }
}