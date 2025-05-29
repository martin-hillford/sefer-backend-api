namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class IsLessonValidRequest(Lesson entity) : IsValidEntityRequest<Lesson>(entity);