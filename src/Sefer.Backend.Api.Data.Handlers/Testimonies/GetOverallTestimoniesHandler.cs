namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class GetOverallTestimoniesHandler(IServiceProvider serviceProvider)
    : Handler<GetOverallTestimoniesRequest, List<Testimony>>(serviceProvider)
{
    public override async Task<List<Testimony>> Handle(GetOverallTestimoniesRequest request, CancellationToken token)
    {
        var context = GetDataContext();
        var testimonies = context.Testimonies.AsNoTracking().Where(t => t.CourseId == null).OrderByDescending(t => t.CreationDate);
        if (request.Limit is > 0) return await testimonies.Take(request.Limit.Value).ToListAsync(token);
        return await testimonies.ToListAsync(token);
    }
}