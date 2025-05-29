namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class AddCurriculumHandler(IServiceProvider serviceProvider)
    : AddEntityHandler<AddCurriculumRequest, Curriculum>(serviceProvider)
{
    public override async Task<bool> Handle(AddCurriculumRequest request, CancellationToken token)
    {
        var inserted = await base.Handle(request, token);
        if (inserted == false) return false;

        // Create a new revision for the course
        var revision = new CurriculumRevision
        {
            Version = 1,
            Stage = Stages.Edit,
            CurriculumId = request.Entity.Id,
            CreationDate = DateTime.UtcNow
        };

        // insert it in the database
        var valid = await IsValidAsync(revision);
        if (!valid) return false;

        var context = GetAsyncContext();
        return await context.AddAsync(revision, token);
    }
}