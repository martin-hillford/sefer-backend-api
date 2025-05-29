namespace Sefer.Backend.Api.Data.Requests.Testimonies;

public class SearchTestimoniesRequest(string searchTerm) : IRequest<List<Testimony>>
{
    public readonly string SearchTerm = searchTerm;
}