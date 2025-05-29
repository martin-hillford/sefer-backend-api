namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class IsCourseNameUniqueHandler(IServiceProvider serviceProvider)
    : Handler<IsCourseNameUniqueRequest, bool>(serviceProvider)
{
    public override async Task<bool> Handle(IsCourseNameUniqueRequest request, CancellationToken token)
    {
        var name = request.Name?.ToLower().Trim();
        if (string.IsNullOrEmpty(name)) return true;
        var context = GetDataContext();
        var courseExists = await context.Courses
            .AnyAsync(sr => sr.Name.ToLower().Trim() == name && sr.Id != request.CourseId, token);
        return !courseExists;
    }
}