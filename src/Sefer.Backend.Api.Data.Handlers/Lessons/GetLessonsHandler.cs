namespace Sefer.Backend.Api.Data.Handlers.Lessons;

public class GetLessonsHandler(IServiceProvider serviceProvider)
    : Handler<GetLessonsRequest, List<Lesson>>(serviceProvider)
{
    public override async Task<List<Lesson>> Handle(GetLessonsRequest request, CancellationToken token)
    {
        if (request.CourseRevisionId == null) { return new List<Lesson>(); }

        var context = GetDataContext();
        return await context.Lessons
            .AsNoTracking()
            .Where(l => l.CourseRevisionId == request.CourseRevisionId)
            .OrderBy(l => l.SequenceId)
            .ToListAsync(token);
    }
}