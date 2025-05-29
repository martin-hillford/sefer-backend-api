namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonBoolQuestionsRequest(Lesson lesson) : GetLessonContentBlocksRequest<BoolQuestion>(lesson);