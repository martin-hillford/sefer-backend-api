namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class GetTestimoniesByCourseIdHandler(IServiceProvider serviceProvider)
    : Handler<GetTestimoniesByCourseIdRequest, List<Testimony>>(serviceProvider)
{
    public override async Task<List<Testimony>> Handle(GetTestimoniesByCourseIdRequest request, CancellationToken token)
    {
        await using var context = GetDataContext();
        return await context.Testimonies.AsNoTracking().Where(t => t.CourseId == request.CourseId).ToListAsync(token);
    }
}