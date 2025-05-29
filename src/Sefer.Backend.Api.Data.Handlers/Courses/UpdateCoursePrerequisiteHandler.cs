namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class UpdateCoursePrerequisiteHandler(IServiceProvider serviceProvider)
    : Handler<UpdateCoursePrerequisiteRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override async Task<bool> Handle(UpdateCoursePrerequisiteRequest request, CancellationToken token)
    {
        if (request.Entity.CourseId == request.Entity.RequiredCourseId) return false;
        var course = await Send(new GetCourseByIdRequest(request.Entity.CourseId), token);
        var required = await Send(new GetCourseByIdRequest(request.Entity.RequiredCourseId), token);
        if (course == null || required == null) return false;


        var context = GetDataContext();
        var helper = new CoursePrerequisiteHelper(_serviceProvider, context);
        if (helper.CreatesLoop(request.Entity)) return false;

        var entityId = request.Entity.Id;

        var current = context.CoursePrerequisites.SingleOrDefault(c => c.Id == entityId);
        if (current != null) helper.Delete(current);

        var instance = new CoursePrerequisite { CourseId = course.Id, RequiredCourseId = required.Id };
        return await helper.Add(instance);
    }
}