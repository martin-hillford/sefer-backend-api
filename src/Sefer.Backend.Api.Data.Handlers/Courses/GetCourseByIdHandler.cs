namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCourseByIdHandler(IServiceProvider serviceProvider)
    : GetEntityByIdHandler<GetCourseByIdRequest, Course>(serviceProvider)
{
    public override async Task<Course> Handle(GetCourseByIdRequest request, CancellationToken token)
    {
        if (!request.WithRevision) return await base.Handle(request, token);
        await using var context = GetDataContext();

        return await context.Courses
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Include(c => c.CourseRevisions)
            .SingleOrDefaultAsync(token);
    }
}