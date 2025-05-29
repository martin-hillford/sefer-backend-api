namespace Sefer.Backend.Api.Data.Requests.Testimonies;

public class GetOverallTestimoniesRequest(int? limit = 0) : IRequest<List<Testimony>>
{
    public readonly int? Limit = limit;
}