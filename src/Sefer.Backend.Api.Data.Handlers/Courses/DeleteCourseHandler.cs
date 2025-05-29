namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class DeleteCourseHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteCourseRequest, Course>(serviceProvider)
{
    public override async Task<bool> Handle(DeleteCourseRequest request, CancellationToken token)
    {
        var course = await Send(new GetCourseByIdRequest(request.EntityId), token);
        if (course?.IsEditable != true) return false;
        return await base.Handle(request, token);
    }
}