namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class GetCourseRevisionByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetCourseRevisionByIdRequest, CourseRevision>(serviceProvider);