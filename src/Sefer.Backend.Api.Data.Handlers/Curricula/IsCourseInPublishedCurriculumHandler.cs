namespace Sefer.Backend.Api.Data.Handlers.Curricula;

public class IsCourseInPublishedCurriculumHandler(IServiceProvider serviceProvider)
    : Handler<IsCourseInPublishedCurriculumRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsCourseInPublishedCurriculumRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.CurriculumBlockCourses
            .AnyAsync(c => c.CourseId == request.CourseId && c.Block.CurriculumRevision.Stage == Stages.Published, cancellationToken: token);

    }
}