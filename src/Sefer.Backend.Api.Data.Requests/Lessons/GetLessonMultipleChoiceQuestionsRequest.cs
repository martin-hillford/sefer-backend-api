namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonMultipleChoiceQuestionsRequest(Lesson lesson)
    : GetLessonContentBlocksRequest<MultipleChoiceQuestion>(lesson);