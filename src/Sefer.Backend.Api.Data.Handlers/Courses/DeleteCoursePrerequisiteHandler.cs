namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class DeleteCoursePrerequisiteHandler(IServiceProvider serviceProvider)
    : SyncHandler<DeleteCoursePrerequisiteRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override bool Handle(DeleteCoursePrerequisiteRequest request)
    {
        try
        {
            var context = GetDataContext();
            var entity = context.CoursePrerequisites.SingleOrDefault(p => p.Id == request.EntityId);
            if (entity == null) return false;

            var helper = new CoursePrerequisiteHelper(_serviceProvider, context);
            return helper.Delete(entity);
        }
        catch (Exception)
        {
            return false;
        }
    }
}