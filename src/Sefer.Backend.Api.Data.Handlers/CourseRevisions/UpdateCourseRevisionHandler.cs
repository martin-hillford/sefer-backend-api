namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class UpdateCourseRevisionHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateCourseRevisionRequest, CourseRevision>(serviceProvider);