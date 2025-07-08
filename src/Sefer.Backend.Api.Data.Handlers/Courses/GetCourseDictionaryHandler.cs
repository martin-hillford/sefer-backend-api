namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCourseDictionaryHandler(IServiceProvider serviceProvider)
    : Handler<GetCourseDictionaryRequest, List<CourseRevisionDictionaryWord>>(serviceProvider)
{
    public override async Task<List<CourseRevisionDictionaryWord>> Handle(GetCourseDictionaryRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        var words = context.CourseRevisionDictionaryWords.Where(c => c.CourseRevisionId == request.CourseRevisionId);
        return await words.ToListAsync(token);
    }
}