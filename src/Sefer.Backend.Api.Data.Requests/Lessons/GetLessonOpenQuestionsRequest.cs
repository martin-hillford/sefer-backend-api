namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonOpenQuestionsRequest(Lesson lesson) : GetLessonContentBlocksRequest<OpenQuestion>(lesson);