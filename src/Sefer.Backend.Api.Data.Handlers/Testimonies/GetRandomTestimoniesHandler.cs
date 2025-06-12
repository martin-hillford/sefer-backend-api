namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class GetRandomTestimoniesHandler(IServiceProvider serviceProvider)
    : Handler<GetRandomTestimoniesRequest, List<Testimony>>(serviceProvider)
{
    public override async Task<List<Testimony>> Handle(GetRandomTestimoniesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var random = context.Testimonies.AsNoTracking();
        if (request.HomepageOnly) random = random.Where(t => t.CourseId == null);

        if (!context.Database.IsSqlCapableServer()) random = random.ToList().AsQueryable();
        var testimonies = random.RandomElements(request.Count);

        if (!context.Database.IsSqlCapableServer()) return testimonies.ToList();
        return await testimonies.ToListAsync(token);
    }
}