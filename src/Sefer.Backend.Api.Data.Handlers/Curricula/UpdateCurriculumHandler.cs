namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class UpdateCurriculumHandler(IServiceProvider serviceProvider)
    : UpdateEntityHandler<UpdateCurriculumRequest, Curriculum>(serviceProvider)
{
    public override async Task<bool> Handle(UpdateCurriculumRequest request, CancellationToken token)
    {
        var curriculum = await Send(new GetCurriculumByIdRequest(request.Entity.Id, true), token);
        if (curriculum?.IsEditable != true) return false;

        return await base.Handle(request, token);
    }
}