namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class DeleteCurriculumHandler(IServiceProvider serviceProvider)
    : DeleteEntityHandler<DeleteCurriculumRequest, Curriculum>(serviceProvider)
{
    public override async Task<bool> Handle(DeleteCurriculumRequest request, CancellationToken token)
    {
        var curriculum = await Send(new GetCurriculumByIdRequest(request.EntityId), token);
        if (curriculum?.IsEditable != true) return false;

        return await base.Handle(request, token);
    }
}