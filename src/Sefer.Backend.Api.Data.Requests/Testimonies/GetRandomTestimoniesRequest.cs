namespace Sefer.Backend.Api.Data.Requests.Testimonies;

public class GetRandomTestimoniesRequest(int count, bool homepageOnly = false) : IRequest<List<Testimony>>
{
    public readonly int Count = count;
    
    public readonly bool HomepageOnly = homepageOnly;
}