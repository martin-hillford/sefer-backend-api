namespace Sefer.Backend.Api.Data.Requests.Courses;

public class UpdateSingleCoursePropertyRequest(Course entity, string property)
    : UpdateSingleEntityPropertyRequest<Course>(entity, property);