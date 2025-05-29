namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonTextElementsRequest(Lesson lesson) : GetLessonContentBlocksRequest<LessonTextElement>(lesson);