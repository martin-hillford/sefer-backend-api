namespace Sefer.Backend.Api.Data.Requests.CourseRevisions;

public class GetCourseRevisionByIdRequest(int? id) : GetEntityByIdRequest<CourseRevision>(id);