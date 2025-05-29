namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonMediaElementsRequest(Lesson lesson) : GetLessonContentBlocksRequest<MediaElement>(lesson);