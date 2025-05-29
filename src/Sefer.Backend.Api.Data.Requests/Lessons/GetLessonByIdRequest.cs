namespace Sefer.Backend.Api.Data.Requests.Lessons;

public class GetLessonByIdRequest(int? id) : GetEntityByIdRequest<Lesson>(id);