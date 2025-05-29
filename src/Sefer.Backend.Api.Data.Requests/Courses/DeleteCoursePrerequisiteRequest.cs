namespace Sefer.Backend.Api.Data.Requests.Courses;

public class DeleteCoursePrerequisiteRequest(CoursePrerequisite entity)
    : DeleteEntityRequest<CoursePrerequisite>(entity);