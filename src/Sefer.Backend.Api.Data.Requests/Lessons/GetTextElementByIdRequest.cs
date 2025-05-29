namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetTextElementByIdRequest(int? id) : GetEntityByIdRequest<LessonTextElement>(id);