namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCoursesHandler(IServiceProvider serviceProvider)
    : GetEntitiesHandler<GetCoursesRequest, Course>(serviceProvider);