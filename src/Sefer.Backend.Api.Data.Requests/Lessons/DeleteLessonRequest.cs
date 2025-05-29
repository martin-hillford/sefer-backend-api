namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class DeleteLessonRequest(Lesson entity) : DeleteEntityRequest<Lesson>(entity);