namespace Sefer.Backend.Api.Data.Handlers.Testimonies;

public class SearchTestimoniesHandler(IServiceProvider serviceProvider)
    : Handler<SearchTestimoniesRequest, List<Testimony>>(serviceProvider)
{
    private const string SearchQuery =
        @"SELECT * FROM testimonies WHERE content @@ to_tsquery({0}) OR name @@ to_tsquery({0})";

    public override async Task<List<Testimony>> Handle(SearchTestimoniesRequest request, CancellationToken token)
    {
        try
        {
            var context = GetDataContext();
            return await context.Testimonies
                .FromSqlRaw(SearchQuery, request.SearchTerm)
                .OrderBy(t => t.Name)
                .ToListAsync(token);
        }
        // But use a fallback mechanism for databases that don't support the full text search
        catch (Exception) { return await NonRawSearchTestimonies(request.SearchTerm, token); }
    }

    private async Task<List<Testimony>> NonRawSearchTestimonies(string searchTerm, CancellationToken token)
    {
        var context = GetDataContext();
        return await context.Testimonies
            .AsNoTracking()
            .Where(t => t.Content.Contains(searchTerm) || t.Name.Contains(searchTerm))
            .OrderBy(t => t.Name)
            .ToListAsync(token);
    }
}