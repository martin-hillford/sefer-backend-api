namespace Sefer.Backend.Api.Data.Handlers.CourseRevisions;

public class SwitchCourseRevisionToEditHandler(IServiceProvider serviceProvider)
    : Handler<SwitchCourseRevisionToEditRequest, bool>(serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public override async Task<bool> Handle(SwitchCourseRevisionToEditRequest request, CancellationToken token)
    {
        // check if we have a revision that can be switched to edit
        var context = GetDataContext();
        var revision = await context.CourseRevisions.SingleOrDefaultAsync(c => c.Id == request.CourseRevisionId, token);
        if (revision is not { Stage: Stages.Test }) return false;

        // Everything is created already, only update the current revision
        revision.ModificationDate = DateTime.UtcNow;
        revision.Stage = Stages.Edit;
        return context.Update(_serviceProvider, revision);
    }
}