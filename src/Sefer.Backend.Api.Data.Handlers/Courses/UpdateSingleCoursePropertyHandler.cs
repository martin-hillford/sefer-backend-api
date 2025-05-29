namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class UpdateSingleCoursePropertyHandler(IServiceProvider serviceProvider)
    : UpdateSingleEntityPropertyHandler<UpdateSingleCoursePropertyRequest, Course>(serviceProvider);