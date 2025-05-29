namespace Sefer.Backend.Api.Data.Requests.Courses;

public class DeleteCourseRequest(Course entity) : DeleteEntityRequest<Course>(entity);