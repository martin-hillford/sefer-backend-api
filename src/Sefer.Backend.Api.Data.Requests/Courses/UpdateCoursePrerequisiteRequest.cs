namespace Sefer.Backend.Api.Data.Requests.Courses;

public class UpdateCoursePrerequisiteRequest(CoursePrerequisite entity)
    : UpdateEntityRequest<CoursePrerequisite>(entity);