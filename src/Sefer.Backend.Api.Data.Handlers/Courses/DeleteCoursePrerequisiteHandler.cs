namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class DeleteCoursePrerequisiteHandler(IServiceProvider serviceProvider) : Handler<DeleteCoursePrerequisiteRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(DeleteCoursePrerequisiteRequest request, CancellationToken token)
    {
        try
        {
            var context = GetDataContext();
            var entities = await context.CoursePrerequisites
                .Where(c => c.CourseId == request.CourseId && c.RequiredCourseId == request.RequiredCourseId)
                .ToListAsync(token);
            
            if (entities.Count == 0) return true;
            
            context.CoursePrerequisites.RemoveRange(entities);
            await context.SaveChangesAsync(token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}