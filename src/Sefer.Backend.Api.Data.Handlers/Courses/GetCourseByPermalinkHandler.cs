namespace Sefer.Backend.Api.Data.Handlers.Courses;

public class GetCourseByPermalinkHandler(IServiceProvider serviceProvider)
    : Handler<GetCourseByPermalinkRequest, Course>(serviceProvider)
{
    public override async Task<Course> Handle(GetCourseByPermalinkRequest request, CancellationToken token)
    {
        var permalink = request.Permalink?.ToLower().Trim();
        if (string.IsNullOrEmpty(permalink)) return null;
        await using var context = GetDataContext();
        return await context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Permalink == permalink, token);
    }
}